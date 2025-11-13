using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using TechWorldService.Models;
using System.Linq;
using TechWorldService.DTO;
using TechWorldService.Data;

namespace TechWorldService.Controllers
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

        [HttpGet("{id}")]
        public ActionResult<ProductReadDTO> GetProductById(int id)
        {
            var product = _repo.GetProductById(id);
            if (product != null)
                return Ok(_mapper.Map<ProductReadDTO>(product));
            return NotFound();
        }

        [HttpGet("by-global/{globalId}")]
        public ActionResult<ProductReadDTO> GetProductByGlobalId(string globalId)
        {
            if (string.IsNullOrWhiteSpace(globalId)) return BadRequest("GlobalId is required");
            var product = _repo.GetProducts().FirstOrDefault(p => p.GlobalId == globalId);
            if (product != null) return Ok(_mapper.Map<ProductReadDTO>(product));
            return NotFound();
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
                return BadRequest("Invalid request");

            var ok = _repo.ReduceStockByGlobalId(request.GlobalId, request.Quantity);
            if (!ok) return BadRequest("Unable to reduce stock");
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
