using GadgetHubAPI.Models;

namespace GadgetHubAPI.Data
{
    public class CustomerRepo
    {
        private readonly AppDbContext _context;

        public CustomerRepo(AppDbContext context)
        {
            _context = context;
        }

        public bool Save() => _context.SaveChanges() > 0;

        public bool Add(Customer customer)
        {
            _context.Customers.Add(customer);
            return Save();
        }

        public List<Customer> GetAll() => _context.Customers.ToList();

        public Customer GetById(int id) => _context.Customers.FirstOrDefault(c => c.Id == id);

        public bool Update(Customer customer)
        {
            _context.Customers.Update(customer);
            return Save();
        }

        public bool Remove(Customer customer)
        {
            _context.Customers.Remove(customer);
            return Save();
        }
    }
}
