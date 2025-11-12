namespace ElectroComService.DTO
{
    public class QuotationResponse
    {
        public int ProductId { get; set; }
        public string GlobalId { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public decimal Price { get; set; }
        public int AvailableStock { get; set; }
        public string DistributorName { get; set; } = "ElectroCom";
        public string DistributorLocation { get; set; } = "Kandy";
        public DateTime DateTime { get; set; } = DateTime.Now;
    }
}
