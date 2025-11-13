using Microsoft.AspNetCore.Mvc;
using GadgetHubAPI.DTO;
using GadgetHubAPI.Services;

namespace GadgetHubAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuotationController : ControllerBase
    {
        private readonly QuotationService _quotationService;
        private readonly ILogger<QuotationController> _logger;

        public QuotationController(QuotationService quotationService, ILogger<QuotationController> logger)
        {
            _quotationService = quotationService;
            _logger = logger;
        }

        /// <summary>
        /// Get the best quotations (lowest prices) from all distributor APIs
        /// </summary>
        /// <param name="requests">List of products to get quotations for</param>
        /// <returns>List of best quotations with lowest prices</returns>
        [HttpPost("get-best")]
        public async Task<ActionResult<List<QuotationResponseDTO>>> GetBestQuotations([FromBody] List<QuotationRequestDTO> requests)
        {
            try
            {
                _logger.LogInformation("QuotationController: Received request for {Count} products", requests?.Count ?? 0);

                if (requests == null || !requests.Any())
                {
                    _logger.LogWarning("QuotationController: No products provided for quotation");
                    return BadRequest("No products provided for quotation.");
                }

                // Validate request data
                var invalidRequests = requests.Where(r => string.IsNullOrWhiteSpace(r.GlobalId) || r.Quantity <= 0).ToList();
                if (invalidRequests.Any())
                {
                    _logger.LogWarning("QuotationController: Invalid requests found: {Count}", invalidRequests.Count);
                    return BadRequest("Invalid request data. GlobalId is required and Quantity must be greater than 0.");
                }

                _logger.LogInformation("QuotationController: Processing {Count} valid requests", requests.Count);

                // Get best quotations from all distributors
                var bestQuotations = await _quotationService.GetBestQuotationsAsync(requests);
                
                _logger.LogInformation("QuotationController: Returning {Count} best quotations", bestQuotations.Count);

                return Ok(bestQuotations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "QuotationController: Error processing quotation request");
                return StatusCode(500, $"Error processing quotations: {ex.Message}");
            }
        }
    }
}





