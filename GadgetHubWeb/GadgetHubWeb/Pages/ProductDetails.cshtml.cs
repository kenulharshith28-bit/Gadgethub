using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using GadgetHubWeb.Services;
using GadgetHubWeb.Models;

namespace GadgetHubWeb.Pages
{
    public class ProductDetailsModel : PageModel
    {
        private readonly ApiClient _apiClient;
        private readonly CartService _cartService;

        public QuotationResponseViewModel? ProductQuotation { get; set; }
        public string? ErrorMessage { get; set; }

        [BindProperty]
        public int Quantity { get; set; } = 1;

        public ProductDetailsModel(ApiClient apiClient, CartService cartService)
        {
            _apiClient = apiClient;
            _cartService = cartService;
        }

        public async Task OnGetAsync(int id)
        {
            try
            {
                // Get all products first
                var products = await _apiClient.GetProductsAsync();
                
                // Debug: Log the available products and the requested ID
                System.Diagnostics.Debug.WriteLine($"Requested Product ID: {id}");
                System.Diagnostics.Debug.WriteLine($"Available Products Count: {products?.Count ?? 0}");
                
                if (products != null && products.Any())
                {
                    foreach (var p in products.Take(5)) // Log first 5 products
                    {
                        System.Diagnostics.Debug.WriteLine($"Product ID: {p.Id}, Name: {p.Name}, GlobalId: {p.GlobalId}");
                    }
                }
                
                var product = products?.FirstOrDefault(p => p.Id == id);
                
                if (product == null)
                {
                    ErrorMessage = $"Product with ID {id} not found. Available products: {products?.Count ?? 0}";
                    return;
                }

                // Create a fallback quotation from the product data with 10% markup
                var distributorPrice = product.Price; // This is the distributor's price
                var customerPrice = distributorPrice * 1.10m; // Add 10% markup
                
                ProductQuotation = new QuotationResponseViewModel
                {
                    GlobalId = product.GlobalId,
                    ProductName = product.Name,
                    Description = product.Description,
                    ImageUrl = product.ImageUrl,
                    Price = customerPrice, // Show customer price with markup
                    DistributorName = product.DistributorName ?? "GadgetHub",
                    DistributorLocation = "Available from GadgetHub"
                };

                // Try to get quotations from distributors, but don't fail if they're not available
                try
                {
                    var request = new List<QuotationRequestViewModel>
                    {
                        new QuotationRequestViewModel { GlobalId = product.GlobalId, Quantity = 1 }
                    };

                    var quotations = await _apiClient.GetBestQuotationsAsync(request);

                    if (quotations.Any())
                    {
                        // Use the best quotation if available, but apply 10% markup
                        var bestQuotation = quotations.First();
                        var distributorPriceFromAPI = bestQuotation.Price;
                        var customerPriceFromAPI = distributorPriceFromAPI * 1.10m; // Add 10% markup
                        
                        ProductQuotation = new QuotationResponseViewModel
                        {
                            GlobalId = bestQuotation.GlobalId,
                            ProductName = bestQuotation.ProductName,
                            Description = bestQuotation.Description,
                            ImageUrl = bestQuotation.ImageUrl,
                            Price = customerPriceFromAPI, // Show customer price with markup
                            DistributorName = bestQuotation.DistributorName,
                            DistributorLocation = bestQuotation.DistributorLocation
                        };
                    }
                    else
                    {
                        // Keep the fallback quotation we created above
                        ErrorMessage = "Pricing from distributors not available. Showing GadgetHub pricing with 10% markup.";
                    }
                }
                catch (Exception ex)
                {
                    // If quotation service fails, use the fallback
                    ErrorMessage = $"Distributor pricing unavailable. Showing GadgetHub pricing with 10% markup. ({ex.Message})";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"An error occurred while loading the product details: {ex.Message}";
            }
        }

        public IActionResult OnPostAddToCart(string globalId, int quantity)
        {
            Console.WriteLine($"OnPostAddToCart called with globalId: {globalId}, quantity: {quantity}");
            
            try
            {
                // Add to cart
                _cartService.AddToCart(globalId, quantity);
                Console.WriteLine("Product added to cart successfully");
                TempData["SuccessMessage"] = $"Product added to cart successfully! (Quantity: {quantity})";
                
                // Redirect to cart page
                return RedirectToPage("/Cart");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding to cart: {ex.Message}");
                TempData["ErrorMessage"] = "Failed to add product to cart. Please try again.";
                return RedirectToPage();
            }
        }

        public async Task<IActionResult> OnPostPlaceOrderAsync(string globalId, int quantity)
        {
            Console.WriteLine($"OnPostPlaceOrderAsync called with globalId: {globalId}, quantity: {quantity}");
            
            try
            {
                var token = HttpContext.Session.GetString("AuthToken");
                if (string.IsNullOrEmpty(token))
                {
                    TempData["ErrorMessage"] = "You must be logged in to place an order.";
                    return RedirectToPage("/Login");
                }

                // Step 1: Get best quotation for this product
                var request = new List<QuotationRequestViewModel>
                {
                    new QuotationRequestViewModel { GlobalId = globalId, Quantity = quantity }
                };
                var quotations = await _apiClient.GetBestQuotationsAsync(request);
                
                if (!quotations.Any())
                {
                    TempData["ErrorMessage"] = "Unable to get pricing for this product. Please try again.";
                    return RedirectToPage();
                }

                var chosenQuotation = quotations.First();

                // Step 2: Place order in GadgetHub API
                var orderRequest = new OrderRequestViewModel
                {
                    CustomerId = 1, // TODO: replace with logged-in customer
                    Items = new List<OrderItemRequestViewModel>
                    {
                        new OrderItemRequestViewModel
                        {
                            GlobalId = chosenQuotation.GlobalId,
                            Quantity = quantity
                        }
                    }
                };

                var orderResponse = await _apiClient.PlaceOrderAsync(orderRequest);

                // Step 3: Redirect to confirmation page
                return RedirectToPage("/OrderConfirmation", new { orderId = orderResponse.OrderId });
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Failed to place order. Please try again.";
                return RedirectToPage();
            }
        }
    }
}