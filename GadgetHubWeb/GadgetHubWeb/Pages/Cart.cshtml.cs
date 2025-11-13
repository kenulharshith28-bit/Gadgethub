using GadgetHubWeb.Models;
using GadgetHubWeb.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GadgetHubWeb.Pages
{
    public class CartModel : PageModel
    {
        private readonly CartService _cartService;
        private readonly ApiClient _apiClient;

        public List<QuotationResponseViewModel> CartItems { get; set; } = new();
        public decimal Total { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Shipping { get; set; }
        public decimal Tax { get; set; }

        public CartModel(CartService cartService, ApiClient apiClient)
        {
            _cartService = cartService;
            _apiClient = apiClient;
        }

        public async Task OnGetAsync()
        {
            try
            {
                var cart = _cartService.GetCart();
                if (cart.Any())
                {
                    var requests = cart.Select(c => new QuotationRequestViewModel 
                    { 
                        GlobalId = c.GlobalId,
                        Quantity = c.Quantity,
                        Location = "Default"
                    }).Where(r => !string.IsNullOrEmpty(r.GlobalId)).ToList();
                    
                    CartItems = await _apiClient.GetBestQuotationsAsync(requests);

                    foreach (var item in CartItems)
                    {
                        var cartItem = cart.First(c => c.GlobalId == item.GlobalId);
                        item.Quantity = cartItem.Quantity;
                        item.Price *= cartItem.Quantity;
                    }

                    Subtotal = CartItems.Sum(i => i.Price);
                    Shipping = Subtotal > 50 ? 0 : 10; // Free shipping over LKR 50
                    Tax = Subtotal * 0.08m; // 8% tax
                    Total = Subtotal + Shipping + Tax;
                }
            }
            catch (Exception)
            {
                // Log the error and set empty cart to prevent page crash
                CartItems = new List<QuotationResponseViewModel>();
                Total = 0;
                // You might want to add a TempData message here to show error to user
                TempData["ErrorMessage"] = "Unable to load cart items. Please try again later.";
            }
        }

        public async Task<IActionResult> OnPostPlaceOrderAsync()
        {
            var token = HttpContext.Session.GetString("AuthToken");
            if (string.IsNullOrEmpty(token))
            {
                TempData["ErrorMessage"] = "You must be logged in to place an order.";
                return RedirectToPage("/Login");
            }

            var cart = _cartService.GetCart();
            if (!cart.Any())
            {
                TempData["ErrorMessage"] = "Your cart is empty.";
                return RedirectToPage();
            }

            try
            {
                // Set authorization header
                _apiClient.SetAuthToken(token);

                // Create order request from cart
                var orderRequest = new OrderRequestViewModel
                {
                    CustomerId = 1, // TODO: Get from logged-in user
                    Items = cart.Select(c => new OrderItemRequestViewModel
                    {
                        GlobalId = c.GlobalId,
                        Quantity = c.Quantity
                    }).ToList()
                };

                var orderResponse = await _apiClient.PlaceOrderAsync(orderRequest);

                // Clear cart after successful order
                _cartService.ClearCart();

                TempData["SuccessMessage"] = $"Order placed successfully! Order ID: {orderResponse.OrderId}";
                return RedirectToPage("/Products");
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Failed to place order. Please try again.";
                return RedirectToPage();
            }
        }

        public IActionResult OnPostRemoveItem(string globalId)
        {
            _cartService.RemoveFromCart(globalId);
            TempData["SuccessMessage"] = "Item removed from cart.";
            return RedirectToPage();
        }

        public IActionResult OnPostClearCart()
        {
            _cartService.ClearCart();
            TempData["SuccessMessage"] = "Cart cleared.";
            return RedirectToPage();
        }
    }
}
