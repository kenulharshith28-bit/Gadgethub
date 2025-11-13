using System.ComponentModel.DataAnnotations;

namespace TechWorldService.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string GlobalId { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public string Name { get; set; }

        [Required]
        public decimal Price { get; set; }

        public int Stock { get; set; }

        [Required]
        public string Description { get; set; }

        public string Category { get; set; }

        public string ImageUrl { get; set; } = "https://via.placeholder.com/300x200?text=TechWorld+Product";

        public List<Order> Orders { get; set; } = new List<Order>();
    }
}
