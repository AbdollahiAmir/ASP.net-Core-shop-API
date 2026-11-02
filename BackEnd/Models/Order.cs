namespace BackEnd.Models
{
    public enum OrderStatus
    {
        Pending = 0,
        Approved = 1,
        Rejected = 2,
        CancelledByCustomer = 3,
        CancelledByAdmin = 4
    }

    public class Order
    {
        public int Id { get; set; }

        public string? CustomerId { get; set; }
        public AppUser? Customer { get; set; }

        public string? CustomerUserName { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string DeliveryAddress { get; set; } = string.Empty;

        public decimal TotalAmount { get; set; }

        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        public string? DecisionReason { get; set; }

        public DateTime? FinalizedAt { get; set; }

        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();

        public ICollection<OrderSellerFulfillment> SellerFulfillments { get; set; } = new List<OrderSellerFulfillment>();
    }
}
