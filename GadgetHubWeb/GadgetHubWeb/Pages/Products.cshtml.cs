using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using GadgetHubWeb.Services;

namespace GadgetHubWeb.Pages
{
    public class ProductsModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly CartService _cartService;

        public ProductsModel(IHttpClientFactory httpClientFactory, CartService cartService)
        {
            _httpClient = httpClientFactory.CreateClient("GadgetHubAPI");
            _cartService = cartService;
        }

        public List<ProductResponseDTO> Products { get; set; } = new();
        public string ErrorMessage { get; set; }

        [BindProperty]
        public string SelectedGlobalId { get; set; }

        [BindProperty]
        public int Quantity { get; set; }

        public QuotationResponseDTO Quotation { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                // Clear any previous error messages
                TempData.Remove("ErrorMessage");
                
                // Attach JWT token if available
                var token = HttpContext.Session.GetString("AuthToken");
                if (!string.IsNullOrEmpty(token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", token);
                }

                // Fetch product catalog
                var response = await _httpClient.GetAsync("api/product");
                
                if (response.IsSuccessStatusCode)
                {
                    Products = await response.Content.ReadFromJsonAsync<List<ProductResponseDTO>>();
                    // Clear any error messages if products loaded successfully
                    TempData.Remove("ErrorMessage");
                }
                else
                {
                    TempData["ErrorMessage"] = "Unable to load products.";
                }
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Server error fetching products.";
            }

            return Page();
        }

        public async Task<IActionResult> OnPostGetQuotationAsync(int productId)
        {
            var token = HttpContext.Session.GetString("AuthToken");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }

            // Map Id -> GlobalId via product catalog
            var products = await _httpClient.GetFromJsonAsync<List<ProductResponseDTO>>("api/product");
            var globalId = products?.FirstOrDefault(p => p.Id == productId)?.GlobalId;
            SelectedGlobalId = globalId;
            var request = new List<QuotationRequestDTO>
            {
                new QuotationRequestDTO { GlobalId = globalId, Quantity = 1 }
            };

            var response = await _httpClient.PostAsJsonAsync("api/quotation/get-best", request);

            if (response.IsSuccessStatusCode)
            {
                var quotations = await response.Content.ReadFromJsonAsync<List<QuotationResponseDTO>>();
                Quotation = quotations?.FirstOrDefault();
            }
            else
            {
                ErrorMessage = "Failed to fetch quotation.";
            }

            await OnGetAsync(); // reload products
            return Page();
        }

        public async Task<IActionResult> OnPostPlaceOrderAsync(int productId, int quantity)
        {
            var token = HttpContext.Session.GetString("AuthToken");
            if (string.IsNullOrEmpty(token))
            {
                TempData["ErrorMessage"] = "You must be logged in to place an order.";
                return RedirectToPage("/Login");
            }

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var products = await _httpClient.GetFromJsonAsync<List<ProductResponseDTO>>("api/product");
            var globalId = products?.FirstOrDefault(p => p.Id == productId)?.GlobalId;

            var payload = new
            {
                CustomerId = 1,
                Products = new [] { new { GlobalId = globalId!, Quantity = quantity } }
            };

            var response = await _httpClient.PostAsJsonAsync("api/order/place", payload);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Order placed successfully!";
            }
            else
            {
                var err = await response.Content.ReadAsStringAsync();
                TempData["ErrorMessage"] = string.IsNullOrWhiteSpace(err) ? "Failed to place order." : $"Failed to place order. {err}";
            }

            return RedirectToPage("/Products");
        }


        // DTOs
        public class ProductResponseDTO
        {
            public int Id { get; set; }
            public string GlobalId { get; set; }
            public string Name { get; set; }
            public string Description { get; set; } = string.Empty;
            public string ImageUrl { get; set; }
        }

        public class QuotationRequestDTO
        {
            public string GlobalId { get; set; }
            public int Quantity { get; set; }
        }

        public class QuotationResponseDTO
        {
            public string GlobalId { get; set; }
            public string ProductName { get; set; }
            public string ImageUrl { get; set; }
            public decimal Price { get; set; }
            public string DistributorName { get; set; }
            public string DistributorLocation { get; set; }
        }

        public class OrderRequestDTO
        {
            public int CustomerId { get; set; }
            public List<OrderItemRequestDTO> Items { get; set; }
        }

        public class OrderItemRequestDTO
        {
            public string GlobalId { get; set; }
            public int Quantity { get; set; }
        }
    }
}
