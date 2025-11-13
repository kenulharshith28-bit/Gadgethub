using System.ComponentModel.DataAnnotations;

namespace GadgetCentralService.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.Now;

        public decimal Total { get; set; }

        public decimal Discount { get; set; }

        public List<Product> Products { get; set; } = new List<Product>();
    }
}
