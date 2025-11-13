using System.ComponentModel.DataAnnotations;

namespace GadgetHubAPI.Models
{
    public class Customer
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Contact { get; set; } = string.Empty;

        [Required]
        public string Address { get; set; } = string.Empty;

    }
}
