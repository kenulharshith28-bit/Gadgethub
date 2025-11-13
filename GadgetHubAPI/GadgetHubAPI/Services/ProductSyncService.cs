using GadgetHubAPI.Data;
using GadgetHubAPI.DTO;
using GadgetHubAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace GadgetHubAPI.Services
{
    /// <summary>
    /// Service responsible for synchronizing products from distributors to GadgetHub database
    /// </summary>
    public class ProductSyncService
    {
        private readonly AppDbContext _context;
        private readonly HttpClient _httpClient;

        public ProductSyncService(AppDbContext context, HttpClient httpClient)
        {
            _context = context;
            _httpClient = httpClient;
        }

        /// <summary>
        /// Syncs all products from all distributors to GadgetHub database
        /// </summary>
        public async Task<List<Product>> SyncAllProductsFromDistributorsAsync()
        {
            var allProducts = new List<Product>();

            // Get products from each distributor with their names
            var distributors = new List<(string Url, string Name)>
            {
                ("https://localhost:7291/api/product", "TechWorld"),
                ("https://localhost:7099/api/product", "ElectroCom"),
                ("https://localhost:7234/api/product", "GadgetCentral")
            };

            foreach (var (url, distributorName) in distributors)
            {
                try
                {
                    var distributorProducts = await GetProductsFromDistributorAsync(url, distributorName);
                    if (distributorProducts != null)
                    {
                        allProducts.AddRange(distributorProducts);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error syncing products from {distributorName} ({url}): {ex.Message}");
                }
            }

            // Store products in GadgetHub database
            await StoreProductsInDatabaseAsync(allProducts);

            return allProducts;
        }

        /// <summary>
        /// Syncs a specific product by ID from all distributors
        /// </summary>
        public async Task<List<Product>> SyncProductByIdFromDistributorsAsync(int productId)
        {
            var allProducts = new List<Product>();

            // Get product from each distributor with their names
            var distributors = new List<(string Url, string Name)>
            {
                ($"https://localhost:7291/api/product/{productId}", "TechWorld"),
                ($"https://localhost:7099/api/product/{productId}", "ElectroCom"),
                ($"https://localhost:7234/api/product/{productId}", "GadgetCentral")
            };

            foreach (var (url, distributorName) in distributors)
            {
                try
                {
                    var product = await GetProductFromDistributorAsync(url, distributorName);
                    if (product != null)
                    {
                        allProducts.Add(product);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error syncing product {productId} from {distributorName} ({url}): {ex.Message}");
                }
            }

            // Store products in GadgetHub database
            await StoreProductsInDatabaseAsync(allProducts);

            return allProducts;
        }

        /// <summary>
        /// Syncs a product by GlobalId from distributors and stores/updates it in the DB
        /// </summary>
        public async Task<Product?> SyncProductByGlobalIdAsync(string globalId)
        {
            if (string.IsNullOrWhiteSpace(globalId)) return null;

            // Distributor product endpoints with names
            var distributors = new List<(string BaseUrl, string Name)>
            {
                ("https://localhost:7291/api/product", "TechWorld"),
                ("https://localhost:7099/api/product", "ElectroCom"),
                ("https://localhost:7234/api/product", "GadgetCentral")
            };

            ProductResponseDTO? matchedDto = null;
            string? distributorName = null;

            // Try fast path: GET by-global on each distributor
            foreach (var (baseUrl, name) in distributors)
            {
                var byGlobalUrl = $"{baseUrl}/by-global/{globalId}";
                try
                {
                    var response = await _httpClient.GetAsync(byGlobalUrl);
                    if (response.IsSuccessStatusCode)
                    {
                        matchedDto = await response.Content.ReadFromJsonAsync<ProductResponseDTO>();
                        if (matchedDto != null)
                        {
                            distributorName = name;
                            break;
                        }
                    }
                }
                catch
                {
                    // Ignore this distributor and continue
                }
            }

            // Fallback: scan list endpoints if by-global not found anywhere
            if (matchedDto == null)
            {
                foreach (var (baseUrl, name) in distributors)
                {
                    try
                    {
                        var response = await _httpClient.GetAsync(baseUrl);
                        if (!response.IsSuccessStatusCode) continue;

                        var productDtos = await response.Content.ReadFromJsonAsync<List<ProductResponseDTO>>();
                        var candidate = productDtos?.FirstOrDefault(p => p.GlobalId == globalId);
                        if (candidate != null)
                        {
                            matchedDto = candidate;
                            distributorName = name;
                            break;
                        }
                    }
                    catch
                    {
                        // Ignore this distributor and continue
                    }
                }
            }

            if (matchedDto == null) return null;

            var product = MapToProduct(matchedDto, distributorName ?? "Unknown");
            await StoreProductsInDatabaseAsync(new List<Product> { product });

            // Return the stored/updated entity from DB
            return await _context.Products.FirstOrDefaultAsync(p => p.GlobalId == globalId);
        }

        /// <summary>
        /// Gets all products from a specific distributor
        /// </summary>
        private async Task<List<Product>?> GetProductsFromDistributorAsync(string distributorUrl, string distributorName)
        {
            try
            {
                var response = await _httpClient.GetAsync(distributorUrl);
                if (response.IsSuccessStatusCode)
                {
                    var productDtos = await response.Content.ReadFromJsonAsync<List<ProductResponseDTO>>();
                    if (productDtos != null)
                    {
                        return productDtos.Select(dto => MapToProduct(dto, distributorName)).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting products from {distributorName} ({distributorUrl}): {ex.Message}");
            }

            return null;
        }

        /// <summary>
        /// Gets a specific product from a distributor
        /// </summary>
        private async Task<Product?> GetProductFromDistributorAsync(string distributorUrl, string distributorName)
        {
            try
            {
                var response = await _httpClient.GetAsync(distributorUrl);
                if (response.IsSuccessStatusCode)
                {
                    var productDto = await response.Content.ReadFromJsonAsync<ProductResponseDTO>();
                    if (productDto != null)
                    {
                        return MapToProduct(productDto, distributorName);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting product from {distributorName} ({distributorUrl}): {ex.Message}");
            }

            return null;
        }

        /// <summary>
        /// Stores products in GadgetHub database, avoiding duplicates
        /// </summary>
        private async Task StoreProductsInDatabaseAsync(List<Product> products)
        {
            try
            {
                foreach (var product in products)
                {
                    try
                    {
                        // Validate required fields
                        if (string.IsNullOrEmpty(product.GlobalId))
                        {
                            Console.WriteLine($"Skipping product with empty GlobalId: {product.Name}");
                            continue;
                        }

                        if (string.IsNullOrEmpty(product.Name))
                        {
                            Console.WriteLine($"Skipping product with empty Name: {product.GlobalId}");
                            continue;
                        }

                        // Check if product already exists by GlobalId
                        var existingProduct = await _context.Products
                            .FirstOrDefaultAsync(p => p.GlobalId == product.GlobalId);

                        if (existingProduct == null)
                        {
                            // Add new product
                            _context.Products.Add(product);
                            Console.WriteLine($"Added new product: {product.Name} (GlobalId: {product.GlobalId})");
                        }
                        else
                        {
                            // Update existing product with latest information
                            existingProduct.Name = product.Name;
                            existingProduct.Description = product.Description;
                            existingProduct.ImageUrl = product.ImageUrl;
                            existingProduct.Stock = product.Stock;
                            // Only update distributor name if it's not already set or if it's more specific
                            if (string.IsNullOrEmpty(existingProduct.DistributorName) || existingProduct.DistributorName == "GadgetHub")
                            {
                                existingProduct.DistributorName = product.DistributorName;
                            }
                            Console.WriteLine($"Updated existing product: {product.Name} (GlobalId: {product.GlobalId}) from {product.DistributorName}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error processing product {product.Name} (GlobalId: {product.GlobalId}): {ex.Message}");
                        // Continue with other products
                    }
                }

                try
                {
                    await _context.SaveChangesAsync();
                    Console.WriteLine($"Successfully saved {products.Count} products to database");
                }
                catch (Microsoft.EntityFrameworkCore.DbUpdateException dbEx)
                {
                    Console.WriteLine($"Database constraint violation: {dbEx.Message}");
                    if (dbEx.InnerException != null)
                    {
                        Console.WriteLine($"Inner exception: {dbEx.InnerException.Message}");
                    }
                    throw;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving products to database: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                throw;
            }
        }

        /// <summary>
        /// Maps ProductResponseDTO to Product entity
        /// </summary>
        private Product MapToProduct(ProductResponseDTO dto, string distributorName = "GadgetHub")
        {
            return new Product
            {
                GlobalId = dto.GlobalId ?? $"UNKNOWN_{Guid.NewGuid()}",
                Name = dto.Name ?? "Unknown Product",
                Description = dto.Description ?? "No description available",
                ImageUrl = dto.ImageUrl ?? "https://via.placeholder.com/400x300?text=No+Image",
                Stock = 0, // Default stock, will be updated from distributor data
                DistributorName = distributorName
            };
        }

        /// <summary>
        /// Gets all products from GadgetHub database
        /// </summary>
        public async Task<List<Product>> GetAllProductsFromDatabaseAsync()
        {
            return await _context.Products.ToListAsync();
        }

        /// <summary>
        /// Clears all products from GadgetHub database
        /// </summary>
        public async Task ClearAllProductsAsync()
        {
            var products = await _context.Products.ToListAsync();
            _context.Products.RemoveRange(products);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Clears products with duplicate GlobalIds to resolve constraint violations
        /// </summary>
        public async Task ClearDuplicateProductsAsync()
        {
            try
            {
                // Find products with duplicate GlobalIds
                var duplicateGroups = await _context.Products
                    .GroupBy(p => p.GlobalId)
                    .Where(g => g.Count() > 1)
                    .ToListAsync();

                foreach (var group in duplicateGroups)
                {
                    // Keep the first one, remove the rest
                    var productsToRemove = group.Skip(1).ToList();
                    _context.Products.RemoveRange(productsToRemove);
                    Console.WriteLine($"Removed {productsToRemove.Count} duplicate products with GlobalId: {group.Key}");
                }

                await _context.SaveChangesAsync();
                Console.WriteLine("Cleared duplicate products successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error clearing duplicate products: {ex.Message}");
                throw;
            }
        }
    }
}
