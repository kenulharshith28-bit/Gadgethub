using System.Net.Http;
using System.Net.Http.Json;
using GadgetHubWeb.Models;

namespace GadgetHubWeb.Services
{
    public class ApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ApiClient> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ApiClient(HttpClient httpClient, ILogger<ApiClient> logger, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _logger = logger;
            _httpClient.BaseAddress = new Uri("https://localhost:7282/"); // GadgetHub API
            _httpContextAccessor = httpContextAccessor;
        }

        public void SetAuthToken(string token)
        {
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
        }

        public async Task<List<ProductViewModel>> GetProductsAsync()
        {
            try
            {
                _logger.LogInformation("Fetching products from GadgetHub API");
                var result = await _httpClient.GetFromJsonAsync<List<ProductViewModel>>("/api/product");
                _logger.LogInformation("Successfully retrieved {Count} products", result?.Count ?? 0);
                return result ?? new List<ProductViewModel>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting products");
                throw;
            }
        }

        //Place Order Part
        public async Task<OrderResponseViewModel> GetOrderByIdAsync(int orderId)
        {
            var response = await _httpClient.GetAsync($"/api/order/{orderId}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<OrderResponseViewModel>();
        }


        //Track order part
        public async Task<OrderResponseViewModel> TrackOrderAsync(int orderId)
        {
            var response = await _httpClient.GetAsync($"/api/order/track/{orderId}");
            if (!response.IsSuccessStatusCode) return null;

            return await response.Content.ReadFromJsonAsync<OrderResponseViewModel>();
        }



        public async Task<List<QuotationResponseViewModel>> GetBestQuotationsAsync(List<QuotationRequestViewModel> requests)
        {
            try
            {
                _logger.LogInformation("Sending quotation request for {Count} products", requests.Count);
                var response = await _httpClient.PostAsJsonAsync("/api/quotation/get-best", requests);
                
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("API call failed with status {StatusCode}: {Error}", response.StatusCode, errorContent);
                    throw new HttpRequestException($"API call failed with status {response.StatusCode}: {errorContent}");
                }
                
                var result = await response.Content.ReadFromJsonAsync<List<QuotationResponseViewModel>>();
                _logger.LogInformation("Successfully retrieved {Count} quotations", result?.Count ?? 0);
                return result ?? new List<QuotationResponseViewModel>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting best quotations");
                throw;
            }
        }

        public async Task<OrderResponseViewModel> PlaceOrderAsync(OrderRequestViewModel orderRequest)
        {
            try
            {
                _logger.LogInformation("Placing order for customer {CustomerId} with {ItemCount} items", 
                    orderRequest.CustomerId, orderRequest.Items.Count);
                
                // Map to API contract: { CustomerId, Products: [{ GlobalId, Quantity }] }
                var payload = new 
                {
                    CustomerId = orderRequest.CustomerId,
                    Products = orderRequest.Items.Select(i => new { i.GlobalId, i.Quantity }).ToList()
                };
                // Attach JWT from session if available
                var token = _httpContextAccessor.HttpContext?.Session?.GetString("AuthToken");
                if (!string.IsNullOrEmpty(token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                }

                var response = await _httpClient.PostAsJsonAsync("/api/order/place", payload);
                
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Order placement failed with status {StatusCode}: {Error}", response.StatusCode, errorContent);
                    throw new HttpRequestException($"Order placement failed with status {response.StatusCode}: {errorContent}");
                }
                
                var result = await response.Content.ReadFromJsonAsync<OrderResponseViewModel>();
                _logger.LogInformation("Successfully placed order {OrderId}", result?.OrderId);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while placing order");
                throw;
            }
        }

       
    }
}
