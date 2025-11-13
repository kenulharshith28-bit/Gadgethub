using GadgetHubAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace GadgetHubAPI.Data
{
    public class OrderRepo
    {
        private readonly AppDbContext _context;

        public OrderRepo(AppDbContext context)
        {
            _context = context;
        }

        public bool Save() => _context.SaveChanges() > 0;

        public bool Add(Order order)
        {
            _context.Orders.Add(order);
            return Save();
        }

        public Order GetById(int id)
        {
            return _context.Orders
                .Include(o => o.Items)
                .FirstOrDefault(o => o.Id == id);
        }

    }
}
