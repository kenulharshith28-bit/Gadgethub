namespace GadgetHubAPI.DTO
{
    public class OrderResponseDTO
    {
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public string DistributorName { get; set; } = string.Empty;
        public string DistributorLocation { get; set; } = string.Empty;
        public List<OrderItemSummaryDTO> ConfirmedItems { get; set; } = new List<OrderItemSummaryDTO>();
        public OrderSummaryDTO OrderSummary { get; set; } = new OrderSummaryDTO();
    }

    public class OrderSummaryDTO
    {
        public int TotalItems { get; set; }
        public int UniqueProducts { get; set; }
        public int DistributorsUsed { get; set; }
        public decimal ProfitMargin { get; set; }
        public decimal FinalPrice { get; set; }
        public string EstimatedDelivery { get; set; } = string.Empty;
    }

    public class OrderItemSummaryDTO
    {
        public string GlobalId { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
