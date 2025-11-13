namespace GadgetHubWeb.Models
{
    public class ProductViewModel
    {
        public int Id { get; set; }
        public string GlobalId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string DistributorName { get; set; }
    }
}
