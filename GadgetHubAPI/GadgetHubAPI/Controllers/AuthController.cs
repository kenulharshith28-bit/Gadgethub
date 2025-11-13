using GadgetHubAPI.Models;
using GadgetHubAPI.Data;
using Microsoft.AspNetCore.Mvc;
using GadgetHubAPI.DTO;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GadgetHubAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public AuthController(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        // ✅ Register
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            try
            {
                var email = (request.Email ?? string.Empty).Trim().ToLowerInvariant();
                var username = (request.Username ?? string.Empty).Trim();
                var contact = (request.Contact ?? string.Empty).Trim();
                var address = (request.Address ?? string.Empty).Trim();
                
                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(request.Password) || string.IsNullOrEmpty(username) || 
                    string.IsNullOrEmpty(contact) || string.IsNullOrEmpty(address))
                    return BadRequest("Username, Email, Password, Contact, and Address are required.");

                // Enforce password length: minimum 6, maximum 8 characters
                if (request.Password.Length < 6 || request.Password.Length > 8)
                    return BadRequest("Password must be 6 to 8 characters long.");

                // Check if username already exists in Users table
                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower());
                if (existingUser != null)
                    return BadRequest("Username already exists.");

                // Check if email already exists in Customer table
                var existingCustomer = await _context.Customers.FirstOrDefaultAsync(c => c.Email.ToLower() == email);
                if (existingCustomer != null)
                    return BadRequest("Email already registered.");

                // Hash the password
                var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

                // Create new User record
                var user = new Users
                {
                    Username = username,
                    PasswordHash = passwordHash,
                    Role = "Customer" // Default role as Customer
                };

                // Create new Customer record
                var customer = new Customer
                {
                    Name = username, // Username maps to Customer.Name
                    Email = email,
                    Contact = contact,
                    Address = address,
                };

                // Add both records to their respective tables
                _context.Users.Add(user);
                _context.Customers.Add(customer);
                await _context.SaveChangesAsync();

                return Ok(new { Message = "Registration successful" });
            }
            catch (Exception ex)
            {
                // Log the full exception for debugging
                return BadRequest($"Registration failed: {ex.Message}. Inner exception: {ex.InnerException?.Message}");
            }
        }

        // 🔍 Debug endpoint to check database schema
        [HttpGet("debug-schema")]
        public async Task<IActionResult> DebugSchema()
        {
            try
            {
                // Try to query the Users table to see what columns exist
                var users = await _context.Users.Take(1).ToListAsync();
                return Ok(new { 
                    Message = "Users table accessible", 
                    UserCount = users.Count,
                    SampleUser = users.FirstOrDefault()
                });
            }
            catch (Exception ex)
            {
                return BadRequest($"Database error: {ex.Message}. Inner: {ex.InnerException?.Message}");
            }
        }

        // 🔍 Debug endpoint to test Users table insert
        [HttpPost("debug-insert")]
        public async Task<IActionResult> DebugInsert()
        {
            try
            {
                // Try to insert a test user
                var testUser = new Users
                {
                    Username = "testuser",
                    PasswordHash = "testhash",
                    Role = "Customer"
                };

                _context.Users.Add(testUser);
                await _context.SaveChangesAsync();

                return Ok(new { Message = "Test user inserted successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest($"Insert error: {ex.Message}. Inner: {ex.InnerException?.Message}");
            }
        }

        // ✅ Login
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var Username = (request.Username ?? string.Empty).Trim().ToLowerInvariant();

            var Users = await _context.Users
                .FirstOrDefaultAsync(c => c.Username != null && c.Username.ToLower() == Username);

            if (Users == null)
                return Unauthorized("Invalid credentials.");

            var passwordHash = Users.PasswordHash ?? string.Empty;
            var isBcryptHash = passwordHash.StartsWith("$2a$") || passwordHash.StartsWith("$2b$") || passwordHash.StartsWith("$2y$");

            var passwordOk = false;
            if (isBcryptHash)
            {
                passwordOk = BCrypt.Net.BCrypt.Verify(request.Password, passwordHash);
            }
            else if (!string.IsNullOrEmpty(passwordHash) && passwordHash == request.Password)
            {
                // Migrate legacy password to bcrypt
                passwordOk = true;
                Users.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
                await _context.SaveChangesAsync();
            }

            if (!passwordOk)
                return Unauthorized("Invalid credentials.");

            // Get customer information if user is a customer
            Customer? customer = null;
            if (Users.Role?.ToLower() == "customer")
            {
                customer = await _context.Customers
                    .FirstOrDefaultAsync(c => c.Name.ToLower() == Users.Username.ToLower());
            }

            // Generate JWT token
            var token = GenerateJwtToken(Users);

            return Ok(new LoginResponse
            {
                Token = token,
                Username = Users.Username,
                Password = Users.PasswordHash,
                Role = Users.Role,
                CustomerId = customer?.Id ?? 0
            });
        }


        // 🔑 JWT generator
        private string GenerateJwtToken(Users user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim("Username", user.Username),
                new Claim("Password", user.PasswordHash),
                new Claim(ClaimTypes.Role, user.Role ?? "User")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

}
