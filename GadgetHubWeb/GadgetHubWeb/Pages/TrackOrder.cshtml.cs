using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using GadgetHubWeb.Services;
using GadgetHubWeb.Models;

namespace GadgetHubWeb.Pages
{
    public class TrackOrderModel : PageModel
    {
        private readonly ApiClient _apiClient;

        [BindProperty]
        public int OrderId { get; set; }

        public OrderResponseViewModel Order { get; set; }
        public string ErrorMessage { get; set; }

        public TrackOrderModel(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public void OnGet()
        {
            // Show empty page initially
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Order = await _apiClient.TrackOrderAsync(OrderId);
            if (Order == null)
            {
                ErrorMessage = "Order not found.";
            }
            return Page();
        }
    }
}
