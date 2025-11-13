namespace GadgetHubWeb.Models
{
    public class OrderTrackingViewModel
    {
        public int OrderId { get; set; }
        public string Status { get; set; }
        public string DistributorName { get; set; }
        public string DistributorLocation { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
