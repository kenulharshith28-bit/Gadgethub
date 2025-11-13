using System.ComponentModel.DataAnnotations;

namespace GadgetHubAPI.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }   

        [Required]
        public string GlobalId { get; set; }  

        [Required]
        public string Name { get; set; }       

        public string Description { get; set; } 

        public string ImageUrl { get; set; }   
        
        public int Stock { get; set; } = 0;    
        
        public string DistributorName { get; set; } = "GadgetHub"; 
    }
}
