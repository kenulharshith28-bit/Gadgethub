using GadgetCentralService.Models;

namespace GadgetCentralService.Data
{
    public class ProductRepo
    {
        private readonly AppDbContext _context;

        public ProductRepo(AppDbContext context)
        {
            _context = context;
        }

        public bool Save()
        {
            return _context.SaveChanges() > 0;
        }

        public bool Add(Product product)
        {
            _context.Products.Add(product);
            return Save();
        }

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

        public List<Product> GetProducts()
        {
            return _context.Products.ToList();
        }

        public Product GetProductById(int id)
        {
            return _context.Products.FirstOrDefault(p => p.Id == id);
        }

        public bool ReduceStockByGlobalId(string globalId, int quantity)
        {
            if (string.IsNullOrWhiteSpace(globalId) || quantity <= 0) return false;
            var product = _context.Products.FirstOrDefault(p => p.GlobalId == globalId);
            if (product == null) return false;
            if (product.Stock < quantity) return false;
            product.Stock -= quantity;
            return Save();
        }
    }
}
