namespace GadgetHubAPI.Models
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
        public decimal TotalAmount { get; set; }

        public string DistributorName { get; set; }  // New: Track distributor
        public string DistributorLocation { get; set; } // New: Track location

        public string Status { get; set; } = "Pending"; // New: Order status

        public List<OrderItem> Items { get; set; } = new List<OrderItem>();
    }
}
