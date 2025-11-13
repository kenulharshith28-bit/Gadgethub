using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using GadgetHubWeb.Models;
using System.Net.Http.Json;

namespace GadgetHubWeb.Pages.AdminDashboard
{
    public class DashboardModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public DashboardModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("GadgetHubAPI");
        }

        public DashboardStats Stats { get; set; } = new DashboardStats();
        public List<OrderDto> RecentOrders { get; set; } = new List<OrderDto>();
        public List<RecentOrderDto> RecentOrdersList { get; set; } = new List<RecentOrderDto>();

        public async Task OnGet()
        {
            // ✅ Call GadgetHub API endpoints
            Stats = await _httpClient.GetFromJsonAsync<DashboardStats>("api/admin/stats") ?? new DashboardStats();
            RecentOrders = await _httpClient.GetFromJsonAsync<List<OrderDto>>("api/order") ?? new List<OrderDto>();
            RecentOrdersList = await _httpClient.GetFromJsonAsync<List<RecentOrderDto>>("api/admin/recent-orders") ?? new List<RecentOrderDto>();
        }

    }

    public class DashboardStats
    {
        public int Products { get; set; }
        public int Customers { get; set; }
        public int Orders { get; set; }
        public decimal Revenue { get; set; }
    }

    public class OrderDto
    {
        public int Id { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
    }


    public class RecentOrderDto
    {
        public int Id { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public DateTime OrderDate { get; set; }
        public string DistributorName { get; set; } = string.Empty;
        public string DistributorLocation { get; set; } = string.Empty;
        public int ItemCount { get; set; }
    }

}
