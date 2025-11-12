using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using ElectroComService.Data;
using ElectroComService.DTO;

namespace ElectroComService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuotationController : ControllerBase
    {
        private readonly ProductRepo _repo;

        public QuotationController(ProductRepo repo)
        {
            _repo = repo;
        }

        [HttpPost("get-quotation")]
        public ActionResult<List<QuotationResponse>> GetQuotation(List<QuotationRequest> requests)
        {
            var responses = new List<QuotationResponse>();

            foreach (var request in requests)
            {
                var product = _repo.GetProducts().FirstOrDefault(p => p.GlobalId == request.GlobalId);
                if (product != null)
                {
                    responses.Add(new QuotationResponse
                    {
                        ProductId = product.Id,
                        GlobalId = product.GlobalId,
                        ProductName = product.Name,
                        Description = product.Description,
                        ImageUrl = product.ImageUrl,
                        Price = product.Price,
                        AvailableStock = product.Stock,
                        DistributorName = "ElectroCom",
                        DistributorLocation = "Kandy",
                        DateTime = DateTime.Now
                    });
                }
            }
            return Ok(responses);
        }
    }
}
