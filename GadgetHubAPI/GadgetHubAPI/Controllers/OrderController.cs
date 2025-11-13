using AutoMapper;
using GadgetHubAPI.Data;
using GadgetHubAPI.DTO;
using GadgetHubAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;

namespace GadgetHubAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly OrderRepo _repo;
        private readonly IMapper _mapper;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly AppDbContext _context;

        public OrderController(OrderRepo repo, IMapper mapper, IHttpClientFactory httpClientFactory, AppDbContext context)
        {
            _repo = repo;
            _mapper = mapper;
            _httpClientFactory = httpClientFactory;
            _context = context;
        }

        /// <summary>
        /// Place a new order
        /// </summary>
        [HttpPost("place")]
        public async Task<ActionResult<OrderResponseDTO>> PlaceOrder(OrderRequestDTO request)
        {
            if (request == null || request.Products == null || !request.Products.Any())
                return BadRequest("Invalid order request. Products are required.");

            try
            {
                // Create a simple order without quotation processing
                var order = new Order
                {
                    CustomerId = request.CustomerId,
                    TotalAmount = request.Products.Sum(p => p.Price * p.Quantity),
                    Status = "Processing",
                    OrderDate = DateTime.Now,
                    DistributorName = "GadgetHub",
                    DistributorLocation = "Colombo",
                    Items = request.Products.Select(p => new OrderItem
                    {
                        GlobalId = p.GlobalId,
                        ProductName = p.ProductName,
                        Quantity = p.Quantity,
                        UnitPrice = p.Price
                    }).ToList()
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                var response = new OrderResponseDTO
                {
                    OrderId = order.Id,
                    CustomerId = order.CustomerId,
                    CustomerName = "Customer", // You may want to fetch from database
                    TotalAmount = order.TotalAmount,
                    Status = order.Status,
                    OrderDate = order.OrderDate,
                    DistributorName = order.DistributorName,
                    DistributorLocation = order.DistributorLocation,
                    ConfirmedItems = order.Items.Select(i => new OrderItemSummaryDTO
                    {
                        GlobalId = i.GlobalId,
                        ProductName = i.ProductName,
                        Quantity = i.Quantity,
                        UnitPrice = i.UnitPrice
                    }).ToList(),
                    OrderSummary = new OrderSummaryDTO
                    {
                        TotalItems = order.Items.Sum(i => i.Quantity),
                        UniqueProducts = order.Items.Count,
                        DistributorsUsed = 1,
                        FinalPrice = order.TotalAmount,
                        EstimatedDelivery = order.OrderDate.AddDays(3).ToString("MMM dd, yyyy")
                    }
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error processing order: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await _context.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return NotFound();

            // Remove order items first if necessary (cascade handles but explicit for clarity)
            if (order.Items != null && order.Items.Count > 0)
            {
                _context.RemoveRange(order.Items);
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return NoContent();
        }




        


        




        [HttpGet]
        public async Task<ActionResult<List<OrderResponseDTO>>> GetAllOrders()
        {
            try
            {
                var orders = await _context.Orders
                    .Include(o => o.Customer)
                    .Include(o => o.Items)
                    .OrderByDescending(o => o.OrderDate)
                    .ToListAsync();

                var orderDtos = orders.Select(order => new OrderResponseDTO
                {
                    OrderId = order.Id,
                    CustomerId = order.CustomerId,
                    CustomerName = order.Customer?.Name ?? "Unknown",
                    TotalAmount = order.TotalAmount,
                    Status = order.Status,
                    OrderDate = order.OrderDate,
                    DistributorName = order.DistributorName,
                    DistributorLocation = order.DistributorLocation,
                    ConfirmedItems = order.Items.Select(i => new OrderItemSummaryDTO
                    {
                        GlobalId = i.GlobalId,
                        ProductName = i.ProductName,
                        Quantity = i.Quantity,
                        UnitPrice = i.UnitPrice
                    }).ToList(),
                    OrderSummary = new OrderSummaryDTO()
                }).ToList();

                return Ok(orderDtos);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving orders: {ex.Message}");
            }
        }

        [HttpGet("customer/{customerId}")]
        public async Task<ActionResult<List<OrderResponseDTO>>> GetOrdersByCustomerId(int customerId)
        {
            try
            {
                var orders = await _context.Orders
                    .Include(o => o.Customer)
                    .Include(o => o.Items)
                    .Where(o => o.CustomerId == customerId)
                    .OrderByDescending(o => o.OrderDate)
                    .ToListAsync();

                var orderDtos = orders.Select(order => new OrderResponseDTO
                {
                    OrderId = order.Id,
                    CustomerId = order.CustomerId,
                    CustomerName = order.Customer?.Name ?? "Unknown",
                    TotalAmount = order.TotalAmount,
                    Status = order.Status,
                    OrderDate = order.OrderDate,
                    DistributorName = order.DistributorName,
                    DistributorLocation = order.DistributorLocation,
                    ConfirmedItems = order.Items.Select(i => new OrderItemSummaryDTO
                    {
                        GlobalId = i.GlobalId,
                        ProductName = i.ProductName,
                        Quantity = i.Quantity,
                        UnitPrice = i.UnitPrice
                    }).ToList(),
                    OrderSummary = new OrderSummaryDTO
                    {
                        TotalItems = order.Items.Sum(i => i.Quantity),
                        UniqueProducts = order.Items.Count,
                        DistributorsUsed = order.DistributorName?.Split(',').Length ?? 1,
                        FinalPrice = order.TotalAmount,
                        EstimatedDelivery = CalculateEstimatedDeliveryDate(order)?.ToString("MMM dd, yyyy") ?? "TBD"
                    }
                }).ToList();

                return Ok(orderDtos);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving customer orders: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public ActionResult<OrderResponseDTO> GetOrderById(int id)
        {
            var order = _repo.GetById(id);
            if (order == null) return NotFound();

            var response = new OrderResponseDTO
            {
                OrderId = order.Id,
                CustomerId = order.CustomerId,
                CustomerName = "",
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                OrderDate = order.OrderDate,
                DistributorName = order.DistributorName,
                DistributorLocation = order.DistributorLocation,
                ConfirmedItems = order.Items.Select(i => new OrderItemSummaryDTO
                {
                    GlobalId = i.GlobalId,
                    ProductName = i.ProductName,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList(),
                OrderSummary = new OrderSummaryDTO()
            };

            return Ok(response);
        }

        private DateTime? CalculateEstimatedDeliveryDate(Order order)
        {
            // Simple delivery estimation based on distributor location
            var deliveryDays = order.DistributorName switch
            {
                "TechWorld" => 2,    // Colombo - fastest
                "ElectroCom" => 3,   // Kandy - medium
                "GadgetCentral" => 4, // Galle - slowest
                _ => 5 // Default
            };

            return order.OrderDate.AddDays(deliveryDays);
        }

        private List<OrderStatusUpdateDTO> CreateStatusHistory(Order order)
        {
            var history = new List<OrderStatusUpdateDTO>
            {
                new OrderStatusUpdateDTO
                {
                    Status = "Processing",
                    Timestamp = order.OrderDate,
                    Description = "Order received and being processed",
                    UpdatedBy = "System"
                }
            };

            if (order.Status == "Confirmed")
            {
                history.Add(new OrderStatusUpdateDTO
                {
                    Status = "Confirmed",
                    Timestamp = order.OrderDate.AddMinutes(5), // Simulate confirmation time
                    Description = "Order confirmed with distributors",
                    UpdatedBy = "System"
                });
            }

            return history;
        }

        private OrderTrackingSummaryDTO CalculateTrackingSummary(Order order)
        {
            return new OrderTrackingSummaryDTO
            {
                TotalItems = order.Items.Sum(i => i.Quantity),
                UniqueProducts = order.Items.Count,
                DistributorsUsed = order.DistributorName.Split(',').Length,
                CurrentStatus = order.Status,
                NextExpectedAction = GetNextExpectedAction(order.Status),
                IsDelivered = order.Status == "Delivered",
                IsCancelled = order.Status == "Cancelled",
                ProfitMargin = CalculateProfitMargin(order)
            };
        }

        private string GetNextExpectedAction(string status)
        {
            return status switch
            {
                "Processing" => "Order confirmation",
                "Confirmed" => "Order preparation",
                "Shipped" => "Out for delivery",
                "Delivered" => "Order completed",
                "Cancelled" => "No further action",
                _ => "Status update pending"
            };
        }

        private decimal CalculateProfitMargin(Order order)
        {
            // Simple profit margin calculation (assuming 10% markup)
            var originalCost = order.TotalAmount / 1.10m;
            return order.TotalAmount - originalCost;
        }
    }
}
