namespace GadgetHubAPI.DTO
{
    public class QuotationResponseDTO
    {
        public int ProductId { get; set; }
        public string GlobalId { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int AvailableStock { get; set; }
        public string DistributorName { get; set; } = string.Empty;
        public string DistributorLocation { get; set; } = string.Empty;
        public DateTime DateTime { get; set; }
    }
}





