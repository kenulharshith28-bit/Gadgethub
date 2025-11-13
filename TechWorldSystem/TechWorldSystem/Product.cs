using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechWorldSystem
{
    internal class Product
    {
        
        public int Id { get; set; }

       
        public string GlobalId { get; set; }

        
        public string Name { get; set; }

        
        public decimal Price { get; set; }

        public int Stock { get; set; }

        public string Description { get; set; }

        public string Category { get; set; }

        public string ImageUrl { get; set; } = "https://via.placeholder.com/300x200?text=TechWorld+Product";

    }
}
