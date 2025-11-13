using GadgetHubWeb.Models;
using GadgetHubWeb.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace GadgetHubWeb.Pages
{
    public class OrdersModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public OrdersModel(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public List<OrderResponseDTO> Orders { get; set; } = new List<OrderResponseDTO>();
        public bool IsLoading { get; set; } = true;
        public string ErrorMessage { get; set; } = string.Empty;

        public async Task<IActionResult> OnGetAsync()
        {
            // Check if user is logged in
            var authToken = HttpContext.Session.GetString("AuthToken");
            var customerId = HttpContext.Session.GetString("CustomerId");

            if (string.IsNullOrEmpty(authToken) || string.IsNullOrEmpty(customerId))
            {
                TempData["ErrorMessage"] = "Please log in to view your orders.";
                return RedirectToPage("/Login");
            }

            try
            {
                await LoadCustomerOrders(int.Parse(customerId));
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error loading orders: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }

            return Page();
        }

        private async Task LoadCustomerOrders(int customerId)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient("GadgetHubAPI");
                var response = await httpClient.GetAsync($"api/Order/customer/{customerId}");

                if (response.IsSuccessStatusCode)
                {
                    var jsonContent = await response.Content.ReadAsStringAsync();
                    var orders = JsonSerializer.Deserialize<List<OrderResponseDTO>>(jsonContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    Orders = orders ?? new List<OrderResponseDTO>();
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ErrorMessage = $"Failed to load orders: {errorContent}";
                    Orders = new List<OrderResponseDTO>();
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error connecting to server: {ex.Message}";
                Orders = new List<OrderResponseDTO>();
            }
        }
    }

    public class OrderResponseDTO
    {
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public string DistributorName { get; set; } = string.Empty;
        public string DistributorLocation { get; set; } = string.Empty;
        public List<OrderItemDTO> ConfirmedItems { get; set; } = new List<OrderItemDTO>();
        public OrderSummaryDTO OrderSummary { get; set; } = new OrderSummaryDTO();
    }

    public class OrderItemDTO
    {
        public string GlobalId { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }

    public class OrderSummaryDTO
    {
        public int TotalItems { get; set; }
        public int UniqueProducts { get; set; }
        public int DistributorsUsed { get; set; }
        public decimal ProfitMargin { get; set; }
        public decimal FinalPrice { get; set; }
        public string EstimatedDelivery { get; set; } = string.Empty;
    }
}






