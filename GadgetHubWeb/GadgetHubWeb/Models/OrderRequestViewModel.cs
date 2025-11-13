namespace GadgetHubWeb.Models
{
    public class OrderRequestViewModel
    {
        public int CustomerId { get; set; }   // Or use customer details if registering on checkout
        public List<OrderItemRequestViewModel> Items { get; set; } = new();
    }

    public class OrderItemRequestViewModel
    {
        public string GlobalId { get; set; }
        public int Quantity { get; set; }
    }
}
