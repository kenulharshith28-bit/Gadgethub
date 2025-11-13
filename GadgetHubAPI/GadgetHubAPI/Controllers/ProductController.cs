using AutoMapper;
using GadgetHubAPI.Data;
using GadgetHubAPI.DTO;
using GadgetHubAPI.Models;
using GadgetHubAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace GadgetHubAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ProductRepo _repo;
        private readonly IMapper _mapper;
        private readonly ProductSyncService _syncService;

        public ProductController(ProductRepo repo, IMapper mapper, ProductSyncService syncService)
        {
            _repo = repo;
            _mapper = mapper;
            _syncService = syncService;
        }

        [HttpPost("sync-by-globalId")]
        public async Task<ActionResult<ProductResponseDTO>> SyncByGlobalId([FromBody] string globalId)
        {
            if (string.IsNullOrWhiteSpace(globalId)) return BadRequest("GlobalId is required");

            var product = await _syncService.SyncProductByGlobalIdAsync(globalId);
            if (product == null) return NotFound($"No product found for GlobalId {globalId}");

            return Ok(_mapper.Map<ProductResponseDTO>(product));
        }

        [HttpPost("add-by-global-id")]
        public async Task<ActionResult<ProductResponseDTO>> AddProductByGlobalId([FromBody] AddProductByGlobalIdRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.GlobalId)) 
                return BadRequest("GlobalId is required");

            try
            {
                // First, sync the product from external services
                var product = await _syncService.SyncProductByGlobalIdAsync(request.GlobalId);
                if (product == null) 
                    return NotFound($"No product found for GlobalId {request.GlobalId}");

                // Check if product already exists in GadgetHub database
                var existingProduct = _repo.GetByGlobalId(request.GlobalId);
                if (existingProduct != null)
                    return BadRequest($"Product with GlobalId {request.GlobalId} already exists in your database.");

                // Add the product to GadgetHub database
                var addedProduct = _repo.Add(product);
                if (addedProduct)
                {
                    var savedProduct = _repo.GetByGlobalId(request.GlobalId);
                    return Ok(_mapper.Map<ProductResponseDTO>(savedProduct));
                }

                return BadRequest("Failed to add product to database.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error adding product: {ex.Message}");
            }
        }


        


        //.
        [HttpPost]
        public ActionResult AddProduct(ProductRequestDTO dto)
        {
            var product = _mapper.Map<Product>(dto);
            if (_repo.Add(product))
                return Ok(_mapper.Map<ProductResponseDTO>(product));
            return BadRequest("Failed to add product.");
        }

        [HttpGet]
        public async Task<ActionResult<List<ProductResponseDTO>>> GetAllProducts()
        {
            // Return only products already in GadgetHub DB (no auto-sync)
            var products = _repo.GetAll();
            return Ok(_mapper.Map<List<ProductResponseDTO>>(products));
        }

        [HttpDelete("by-global-id/{globalId}")]
        public ActionResult DeleteProductByGlobalId(string globalId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(globalId))
                    return BadRequest(new { message = "GlobalId is required" });

                var product = _repo.GetByGlobalId(globalId);
                if (product == null)
                    return NotFound(new { message = $"Product with GlobalId {globalId} not found." });

                if (_repo.Delete(product.Id))
                    return Ok(new { message = $"Product '{product.Name}' deleted successfully." });

                return BadRequest(new { message = "Failed to delete product." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error deleting product: {ex.Message}" });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductResponseDTO>> GetProductById(int id)
        {
            var product = _repo.GetById(id);
            if (product == null) return NotFound();
            return Ok(_mapper.Map<ProductResponseDTO>(product));
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteProduct(int id)
        {
            try
            {
                var product = _repo.GetById(id);
                if (product == null)
                    return NotFound(new { message = $"Product with ID {id} not found." });

                if (_repo.Delete(id))
                    return Ok(new { message = $"Product '{product.Name}' deleted successfully." });

                return BadRequest(new { message = "Failed to delete product." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error deleting product: {ex.Message}" });
            }
        }

        [HttpGet("stock/{globalId}")]
        public async Task<ActionResult<object>> GetProductStock(string globalId)
        {
            if (string.IsNullOrWhiteSpace(globalId))
                return BadRequest("GlobalId is required");

            var product = _repo.GetByGlobalId(globalId);
            if (product == null)
                return NotFound($"Product with GlobalId {globalId} not found in GadgetHub inventory.");

            return Ok(new
            {
                GlobalId = product.GlobalId,
                ProductName = product.Name,
                CurrentStock = product.Stock,
                Description = product.Description,
                ImageUrl = product.ImageUrl,
                LastUpdated = DateTime.Now
            });
        }

        [HttpGet("stock")]
        public async Task<ActionResult<List<object>>> GetAllProductsStock()
        {
            var products = _repo.GetAll();
            var stockInfo = products.Select(p => new
            {
                GlobalId = p.GlobalId,
                ProductName = p.Name,
                CurrentStock = p.Stock,
                Description = p.Description,
                ImageUrl = p.ImageUrl,
                LastUpdated = DateTime.Now
            }).ToList();

            return Ok(stockInfo);
        }

        [HttpPost("sync-all-distributors")]
        public async Task<ActionResult<List<ProductResponseDTO>>> SyncAllDistributors()
        {
            try
            {
                var products = await _syncService.SyncAllProductsFromDistributorsAsync();
                return Ok(_mapper.Map<List<ProductResponseDTO>>(products));
            }
            catch (Exception ex)
            {
                return BadRequest($"Error syncing products from distributors: {ex.Message}");
            }
        }

        [HttpPost("update-distributor-names")]
        public async Task<ActionResult> UpdateDistributorNames()
        {
            try
            {
                var products = _repo.GetAll();
                var updatedCount = 0;

                foreach (var product in products)
                {
                    // Update distributor names based on GlobalId patterns or other logic
                    if (product.GlobalId.StartsWith("TWN"))
                    {
                        product.DistributorName = "TechWorld";
                        updatedCount++;
                    }
                    else if (product.GlobalId.StartsWith("EC"))
                    {
                        product.DistributorName = "ElectroCom";
                        updatedCount++;
                    }
                    else if (product.GlobalId.StartsWith("GC"))
                    {
                        product.DistributorName = "GadgetCentral";
                        updatedCount++;
                    }
                    else if (product.DistributorName == "GadgetHub" || string.IsNullOrEmpty(product.DistributorName))
                    {
                        // For products without clear distributor pattern, try to sync from external sources
                        var syncedProduct = await _syncService.SyncProductByGlobalIdAsync(product.GlobalId);
                        if (syncedProduct != null && syncedProduct.DistributorName != "GadgetHub")
                        {
                            product.DistributorName = syncedProduct.DistributorName;
                            updatedCount++;
                        }
                    }
                }

                // Save changes
                if (updatedCount > 0)
                {
                    _repo.Save();
                }

                return Ok(new { message = $"Updated distributor names for {updatedCount} products" });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error updating distributor names: {ex.Message}");
            }
        }

        
    }
}
