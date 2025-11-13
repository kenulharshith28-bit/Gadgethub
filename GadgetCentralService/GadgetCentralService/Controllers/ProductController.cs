using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using GadgetCentralService.Models;
using GadgetCentralService.DTO;
using GadgetCentralService.Data;
using System.Linq;

namespace GadgetCentralService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ProductRepo _repo;

        public ProductController(IMapper mapper, ProductRepo repo)
        {
            _mapper = mapper;
            _repo = repo;
        }

        [HttpPost]
        public ActionResult AddProduct([FromBody] ProductWriteDTO writeDTO)
        {
            var product = _mapper.Map<Product>(writeDTO);
            if (_repo.Add(product))
                return Ok();
            return BadRequest();
        }

        [HttpGet]
        public ActionResult<List<ProductReadDTO>> GetProducts()
        {
            var products = _repo.GetProducts();
            return Ok(_mapper.Map<List<ProductReadDTO>>(products));
        }

        [HttpGet("ids")]
        public ActionResult<List<int>> GetProductIds()
        {
            var productIds = _repo.GetProducts().Select(p => p.Id).ToList();
            return Ok(productIds);
        }

        [HttpGet("{id}")]
        public ActionResult<ProductReadDTO> GetProductById(int id)
        {
            if (id <= 0)
                return BadRequest(new { message = "Product ID must be greater than 0" });

            var product = _repo.GetProductById(id);
            if (product != null)
                return Ok(_mapper.Map<ProductReadDTO>(product));
            
            return NotFound(new { message = $"Product with ID {id} not found" });
        }

        [HttpGet("by-global/{globalId}")]
        public ActionResult<ProductReadDTO> GetProductByGlobalId(string globalId)
        {
            if (string.IsNullOrWhiteSpace(globalId))
                return BadRequest(new { message = "GlobalId is required" });

            var product = _repo.GetProducts().FirstOrDefault(p => p.GlobalId == globalId);
            if (product != null)
                return Ok(_mapper.Map<ProductReadDTO>(product));

            return NotFound(new { message = $"Product with GlobalId {globalId} not found" });
        }

        [HttpPut("{id}")]
        public ActionResult Update(int id, ProductWriteDTO writeDTO)
        {
            var product = _mapper.Map<Product>(writeDTO);
            product.Id = id;
            if (_repo.Update(product))
                return Ok();
            return NotFound();
        }

        [HttpPost("reduce-stock")]
        public ActionResult ReduceStock([FromBody] ReduceStockRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.GlobalId) || request.Quantity <= 0)
                return BadRequest(new { message = "Invalid request" });

            var ok = _repo.ReduceStockByGlobalId(request.GlobalId, request.Quantity);
            if (!ok) return BadRequest(new { message = "Unable to reduce stock" });
            return Ok();
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            var product = _repo.GetProductById(id);
            if (product != null)
            {
                _repo.Remove(product);
                return Ok();
            }
            return NotFound();
        }
    }
}
