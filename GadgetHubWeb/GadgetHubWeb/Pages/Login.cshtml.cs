using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace GadgetHubWeb.Pages
{
    public class LoginModel(IHttpClientFactory httpClientFactory) : PageModel
    {
        private readonly HttpClient _httpClient = httpClientFactory.CreateClient("GadgetHubAPI");

        [BindProperty]
        public LoginRequestDTO LoginRequest { get; set; } = new();

        public string ErrorMessage { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/login", LoginRequest);

            if (!response.IsSuccessStatusCode)
            {
                ErrorMessage = "Invalid username or password.";
                return Page();
            }

            var result = await response.Content.ReadFromJsonAsync<LoginResponseDTO>();

            if (result != null && !string.IsNullOrEmpty(result.Token))
            {
                // Store token in session
                HttpContext.Session.SetString("AuthToken", result.Token);
                if (!string.IsNullOrEmpty(result.Username))
                {
                    HttpContext.Session.SetString("UserName", result.Username);
                }
                if (!string.IsNullOrEmpty(result.Role))
                {
                    HttpContext.Session.SetString("UserRole", result.Role);
                }
                if (result.CustomerId > 0)
                {
                    HttpContext.Session.SetString("CustomerId", result.CustomerId.ToString());
                }

                // Redirect based on user role
                if (result.Role?.ToLower() == "admin")
                {
                    return RedirectToPage("/AdminDashboard");
                }
                else
                {
                    return RedirectToPage("/Index");
                }
            }

            ErrorMessage = "Login failed. Please try again.";
            return Page();
        }
    }

    public class LoginRequestDTO
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class LoginResponseDTO
    {
        public string Token { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public int CustomerId { get; set; }
    }
}
