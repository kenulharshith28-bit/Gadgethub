namespace GadgetHubWeb.Models
{
    public class OrderResponseViewModel
    {
        public int OrderId { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public string DistributorName { get; set; }
        public string DistributorLocation { get; set; }
        public DateTime OrderDate { get; set; }
    }
}
