using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Json;

namespace GadgetHubWeb.Pages
{
    public class AddProductModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public AddProductModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("GadgetHubAPI");
        }

        [BindProperty]
        public string GlobalId { get; set; } = string.Empty;

        public string SuccessMessage { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrWhiteSpace(GlobalId))
            {
                ErrorMessage = "Global ID is required.";
                return Page();
            }

            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/product/add-by-global-id", new { GlobalId = GlobalId.Trim() });

                if (response.IsSuccessStatusCode)
                {
                    SuccessMessage = "Product added successfully to your database!";
                    GlobalId = string.Empty; // Clear form
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    
                    // Check if the error is about product already existing
                    if (errorContent.Contains("already exists"))
                    {
                        SuccessMessage = "Product Added Successfully";
                        GlobalId = string.Empty; // Clear form
                    }
                    else
                    {
                        ErrorMessage = $"Failed to add product: {errorContent}";
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"An error occurred: {ex.Message}";
            }

            return Page();
        }
    }
}






