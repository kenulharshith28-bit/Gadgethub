namespace GadgetHubWeb.Models
{
    public class QuotationResponseViewModel
    {
        public string GlobalId { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public decimal Price { get; set; }
        public string DistributorName { get; set; }
        public string DistributorLocation { get; set; }
        public int Quantity { get; set; }
    }
}
