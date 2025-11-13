using System.ComponentModel.DataAnnotations;

namespace GadgetCentralService.DTO
{
    public class ProductWriteDTO
    {
        [Required]
        public string GlobalId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public decimal Price { get; set; }

        public int Stock { get; set; }

        [Required]
        public string Description { get; set; }

        public string Category { get; set; }

        public string ImageUrl { get; set; }
    }
}
