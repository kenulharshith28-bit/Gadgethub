namespace GadgetHubAPI.DTO
{
    public class OrderRequestDTO
    {
        public int CustomerId { get; set; }
        public List<OrderItemRequestDTO> Products { get; set; }
    }

    public class OrderItemRequestDTO
    {
        public string GlobalId { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
