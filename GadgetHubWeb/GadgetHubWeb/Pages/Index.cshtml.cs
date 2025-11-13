using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using GadgetHubWeb.Services;
using GadgetHubWeb.Models;

namespace GadgetHubWeb.Pages
{
    public class IndexModel(ApiClient apiClient, CartService cartService) : PageModel
    {
        private readonly ApiClient _apiClient = apiClient;
        private readonly CartService _cartService = cartService;
        public List<ProductViewModel> Products { get; set; }

        public async Task OnGetAsync()
        {
            Products = await _apiClient.GetProductsAsync();
        }

        public IActionResult OnPostAddToCart(string globalId, int quantity)
        {
            _cartService.AddToCart(globalId, quantity);
            return RedirectToPage("/Cart");
        }
    }
}
