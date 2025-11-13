using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Json;

namespace GadgetHubWeb.Pages
{
    public class AdminRegisterModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public AdminRegisterModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("GadgetHubAPI");
        }

        [BindProperty]
        public AdminRegisterRequest AdminRegisterRequest { get; set; } = new();

        public string SuccessMessage { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/admin/register-admin", AdminRegisterRequest);

                if (response.IsSuccessStatusCode)
                {
                    SuccessMessage = "Admin registered successfully!";
                    AdminRegisterRequest = new AdminRegisterRequest(); // Clear form
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ErrorMessage = $"Registration failed: {errorContent}";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"An error occurred: {ex.Message}";
            }

            return Page();
        }
    }

    public class AdminRegisterRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}







