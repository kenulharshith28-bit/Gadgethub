using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace GadgetHubWeb.Pages
{
    public class RegisterModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public RegisterModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("GadgetHubAPI");
        }

        [BindProperty]
        public RegisterRequestDTO RegisterRequest { get; set; } = new();

        public string ErrorMessage { get; set; }
        public string SuccessMessage { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/register", RegisterRequest);

            if (response.IsSuccessStatusCode)
            {
                SuccessMessage = "Registration successful! You can now log in.";
                return RedirectToPage("/Login");
            }

            // Show server-provided reason if available (e.g., Email already registered.)
            try
            {
                var apiError = await response.Content.ReadAsStringAsync();
                ErrorMessage = string.IsNullOrWhiteSpace(apiError) ? "Failed to register. Please try again." : apiError.Trim('"');
            }
            catch
            {
                ErrorMessage = "Failed to register. Please try again.";
            }
            return Page();
        }
    }

    public class RegisterRequestDTO
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Contact { get; set; }
        public string Address { get; set; }
    }
}
