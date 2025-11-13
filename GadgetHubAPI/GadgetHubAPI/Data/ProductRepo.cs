using GadgetHubAPI.Models;

namespace GadgetHubAPI.Data
{
    public class ProductRepo
    {
        private readonly AppDbContext _context;

        public ProductRepo(AppDbContext context)
        {
            _context = context;
        }

        public bool Save() => _context.SaveChanges() > 0;

        public bool Add(Product product)
        {
            _context.Products.Add(product);
            return Save();
        }

        public List<Product> GetAll() => _context.Products.ToList();

        public Product GetById(int id) => _context.Products.FirstOrDefault(p => p.Id == id);

        public Product GetByGlobalId(string globalId) => _context.Products.FirstOrDefault(p => p.GlobalId == globalId);

        public bool Update(Product product)
        {
            _context.Products.Update(product);
            return Save();
        }

        public bool Remove(Product product)
        {
            _context.Products.Remove(product);
            return Save();
        }

        public bool Delete(int id)
        {
            var product = GetById(id);
            if (product == null) return false;
            
            _context.Products.Remove(product);
            return Save();
        }
    }
}
