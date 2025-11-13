using Microsoft.AspNetCore.Mvc;
using GadgetHubAPI.Data;
using GadgetHubAPI.DTO;
using GadgetHubAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace GadgetHubAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            try
            {
                var stats = new
                {
                    Products = await _context.Products.CountAsync(),
                    Customers = await _context.Customers.CountAsync(),
                    Orders = await _context.Orders.CountAsync(),
                    Revenue = await _context.Orders.SumAsync(o => o.TotalAmount)
                };

                return Ok(stats);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving stats: {ex.Message}");
            }
        }

        [HttpGet("recent-orders")]
        public async Task<IActionResult> GetRecentOrders()
        {
            try
            {
                var recentOrders = await _context.Orders
                    .Include(o => o.Customer)
                    .Include(o => o.Items)
                    .OrderByDescending(o => o.OrderDate)
                    .Take(10) // Get last 10 orders
                    .Select(order => new
                    {
                        Id = order.Id,
                        CustomerName = order.Customer.Name,
                        Status = order.Status,
                        TotalAmount = order.TotalAmount,
                        OrderDate = order.OrderDate,
                        DistributorName = order.DistributorName,
                        DistributorLocation = order.DistributorLocation,
                        ItemCount = order.Items.Count
                    })
                    .ToListAsync();

                return Ok(recentOrders);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving recent orders: {ex.Message}");
            }
        }

        [HttpPost("register-admin")]
        public async Task<IActionResult> RegisterAdmin(AdminRegisterRequest request)
        {
            try
            {
                var username = (request.Username ?? string.Empty).Trim();
                
                if (string.IsNullOrEmpty(request.Password) || string.IsNullOrEmpty(username))
                    return BadRequest("Username and Password are required.");

                // Enforce password length: minimum 6, maximum 8 characters
                if (request.Password.Length < 6 || request.Password.Length > 8)
                    return BadRequest("Password must be 6 to 8 characters long.");

                // Check if username already exists in Users table
                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower());
                if (existingUser != null)
                    return BadRequest("Username already exists.");

                // Hash the password
                var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

                // Create new User record with Admin role
                var user = new Users
                {
                    Username = username,
                    PasswordHash = passwordHash,
                    Role = "Admin" // Set role as Admin
                };

                // Add user to Users table only
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return Ok(new { Message = "Admin registered successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest($"Admin registration failed: {ex.Message}");
            }
        }
    }
}
