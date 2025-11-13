using System.ComponentModel.DataAnnotations;

namespace GadgetHubAPI.Models
{
    public class OrderItem
    {
        [Key] 
        public int Id { get; set; }
        public string GlobalId { get; set; }
        public string ProductName { get; set; }

        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
