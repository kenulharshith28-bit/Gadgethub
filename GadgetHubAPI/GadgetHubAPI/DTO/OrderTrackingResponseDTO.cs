namespace GadgetHubAPI.DTO
{
    public class OrderTrackingResponseDTO
    {
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string DistributorName { get; set; } = string.Empty;
        public string DistributorLocation { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public DateTime? EstimatedDeliveryDate { get; set; }
        public decimal TotalAmount { get; set; }
        public List<OrderItemResponseDTO> Items { get; set; } = new List<OrderItemResponseDTO>();
        public List<OrderStatusUpdateDTO> StatusHistory { get; set; } = new List<OrderStatusUpdateDTO>();
        public OrderTrackingSummaryDTO TrackingSummary { get; set; } = new OrderTrackingSummaryDTO();
    }

    public class OrderItemResponseDTO
    {
        public string GlobalId { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public string DistributorName { get; set; } = string.Empty;
        public string DistributorLocation { get; set; } = string.Empty;
    }

    public class OrderStatusUpdateDTO
    {
        public string Status { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string Description { get; set; } = string.Empty;
        public string UpdatedBy { get; set; } = string.Empty;
    }

    public class OrderTrackingSummaryDTO
    {
        public int TotalItems { get; set; }
        public int UniqueProducts { get; set; }
        public int DistributorsUsed { get; set; }
        public string CurrentStatus { get; set; } = string.Empty;
        public string NextExpectedAction { get; set; } = string.Empty;
        public bool IsDelivered { get; set; }
        public bool IsCancelled { get; set; }
        public decimal ProfitMargin { get; set; }
    }
}
