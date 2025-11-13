using Microsoft.AspNetCore.Mvc.RazorPages;
using GadgetHubWeb.Services;
using GadgetHubWeb.Models;

namespace GadgetHubWeb.Pages
{
    public class OrderConfirmationModel : PageModel
    {
        private readonly ApiClient _apiClient;
        public OrderResponseViewModel Order { get; set; }

        public OrderConfirmationModel(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task OnGetAsync(int orderId)
        {
            // Fetch order details
            Order = await _apiClient.GetOrderByIdAsync(orderId);
        }
    }
}
