using GadgetHubAPI.DTO;
using GadgetHubAPI.Models;
using GadgetHubAPI.Data;

namespace GadgetHubAPI.Services
{
    public class OrderService
    {
        private readonly OrderRepo _orderRepo;
        private readonly AppDbContext _context;

        public OrderService(OrderRepo repo, AppDbContext context)
        {
            _orderRepo = repo;
            _context = context;
        }

        public async Task<Order> CreateOrder(int customerId, List<OrderItemSummaryDTO> orderItems)
        {
            // Validate or create customer
            var validCustomerId = await EnsureCustomerExists(customerId);

            var order = new Order
            {
                CustomerId = validCustomerId,
                TotalAmount = orderItems.Sum(q => q.UnitPrice * q.Quantity),
                Items = orderItems.Select(q => new OrderItem
                {
                    GlobalId = q.GlobalId,
                    ProductName = q.ProductName,
                    Quantity = q.Quantity,
                    UnitPrice = q.UnitPrice
                }).ToList()
            };

            _orderRepo.Add(order);
            return order;
        }

        private async Task<int> EnsureCustomerExists(int customerId)
        {
            // Check if customer exists
            var existingCustomer = await _context.Customers.FindAsync(customerId);
            if (existingCustomer != null)
            {
                return customerId;
            }

            // If customer doesn't exist, create a default customer
            var defaultCustomer = new Customer
            {
                Name = "Default Customer",
                Email = "default@example.com",
                
            };

            _context.Customers.Add(defaultCustomer);
            await _context.SaveChangesAsync();

            return defaultCustomer.Id;
        }
    }
}
