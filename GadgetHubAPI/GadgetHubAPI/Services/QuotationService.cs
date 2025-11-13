using GadgetHubAPI.Data;
using GadgetHubAPI.DTO;
using GadgetHubAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace GadgetHubAPI.Services
{
    public class QuotationService
    {
        private readonly AppDbContext _context;
        private readonly HttpClient _httpClient;

        public QuotationService(AppDbContext context, HttpClient httpClient)
        {
            _context = context;
            _httpClient = httpClient;
            
            // Configure HttpClient to ignore SSL certificate errors in development
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "GadgetHubAPI/1.0");
        }

        /// <summary>
        /// Get the best quotation (lowest price) from all distributor APIs
        /// </summary>
        public async Task<List<QuotationResponseDTO>> GetBestQuotationsAsync(List<QuotationRequestDTO> requests)
        {
            var allQuotations = new List<QuotationResponseDTO>();

            // Distributor API endpoints
            var distributorUrls = new List<string>
            {
                "https://localhost:7291/api/Quotation/get-quotation", // TechWorld
                "https://localhost:7099/api/Quotation/get-quotation", // ElectroCom
                "https://localhost:7234/api/Quotation/get-quotation"  // GadgetCentral
            };

            Console.WriteLine($"QuotationService: Getting quotations for {requests.Count} products");

            // Call each distributor API
            foreach (var url in distributorUrls)
            {
                try
                {
                    Console.WriteLine($"QuotationService: Calling {url}");
                    Console.WriteLine($"QuotationService: Request payload: {System.Text.Json.JsonSerializer.Serialize(requests)}");
                    
                    var response = await _httpClient.PostAsJsonAsync(url, requests);
                    Console.WriteLine($"QuotationService: Response status: {response.StatusCode}");
                    
                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        Console.WriteLine($"QuotationService: Response content: {responseContent}");
                        
                        var quotations = await response.Content.ReadFromJsonAsync<List<QuotationResponseDTO>>();
                        if (quotations != null && quotations.Any())
                        {
                            Console.WriteLine($"QuotationService: Received {quotations.Count} quotations from {url}");
                            allQuotations.AddRange(quotations);
                        }
                        else
                        {
                            Console.WriteLine($"QuotationService: No quotations in response from {url}");
                        }
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        Console.WriteLine($"QuotationService: Error from {url}: {response.StatusCode} - {errorContent}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"QuotationService: Exception calling {url}: {ex.Message}");
                    Console.WriteLine($"QuotationService: Stack trace: {ex.StackTrace}");
                }
            }

            Console.WriteLine($"QuotationService: Total quotations received: {allQuotations.Count}");

            // Select the best quotation (lowest price) for each product
            var bestQuotations = new List<QuotationResponseDTO>();
            
            foreach (var request in requests)
            {
                var productQuotations = allQuotations
                    .Where(q => q.GlobalId == request.GlobalId)
                    .ToList();

                if (productQuotations.Any())
                {
                    // Select the quotation with the lowest price
                    var bestQuotation = productQuotations
                        .OrderBy(q => q.Price)
                        .First();

                    // Enhance with GadgetHub product data if available
                    var product = await _context.Products
                        .FirstOrDefaultAsync(p => p.GlobalId == request.GlobalId);

                    if (product != null)
                    {
                        bestQuotation.ProductName = product.Name;
                        bestQuotation.Description = product.Description;
                        bestQuotation.ImageUrl = product.ImageUrl;
                    }

                    bestQuotations.Add(bestQuotation);
                    Console.WriteLine($"QuotationService: Best quotation for {request.GlobalId}: {bestQuotation.DistributorName} - ${bestQuotation.Price}");
                }
                else
                {
                    Console.WriteLine($"QuotationService: No quotations found for {request.GlobalId}");
                }
            }

            Console.WriteLine($"QuotationService: Returning {bestQuotations.Count} best quotations");
            return bestQuotations;
        }
    }
}
