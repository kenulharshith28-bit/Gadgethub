using System.ComponentModel.DataAnnotations;

namespace TechWorldService.Models
{
    public class Quotation
    {
        [Key]
        public int Id { get; set; }

        public string ProductName { get; set; }

        public decimal Price { get; set; }

        public DateTime DateTime { get; set; } = DateTime.Now;

        public string DistributorName { get; set; } = "TechWorld";
    }
}
