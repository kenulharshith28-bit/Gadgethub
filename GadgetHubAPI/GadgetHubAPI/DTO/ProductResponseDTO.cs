namespace GadgetHubAPI.DTO
{
    public class ProductResponseDTO
    {
        public int Id { get; set; }
        public string GlobalId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public decimal Price { get; set; }   // Product price
        public int Stock { get; set; }       // Current stock level in GadgetHub
        public string DistributorName { get; set; } // Distributor name
    }
}
