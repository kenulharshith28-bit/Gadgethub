using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Json;

namespace GadgetHubWeb.Pages
{
    public class ProductListModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public ProductListModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("GadgetHubAPI");
        }

        public List<ProductDto> Products { get; set; } = new List<ProductDto>();
        public int TotalProducts { get; set; }
        public int InStockProducts { get; set; }
        public int LowStockProducts { get; set; }
        public int OutOfStockProducts { get; set; }

        public async Task OnGetAsync()
        {
            try
            {
                Products = await _httpClient.GetFromJsonAsync<List<ProductDto>>("api/product") ?? new List<ProductDto>();
                
                // Calculate statistics
                TotalProducts = Products.Count;
                InStockProducts = Products.Count(p => p.Stock > 10);
                LowStockProducts = Products.Count(p => p.Stock > 0 && p.Stock <= 10);
                OutOfStockProducts = Products.Count(p => p.Stock == 0);
            }
            catch (Exception)
            {
                // Log error if needed
                Products = new List<ProductDto>();
            }
        }
    }

    public class ProductDto
    {
        public int Id { get; set; }
        public string GlobalId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string DistributorName { get; set; } = string.Empty;
    }
}

