namespace BackEnd.Models
{
    public enum SellerFulfillmentStatus
    {
        Pending = 0,
        UnableToSupply = 1,
        SubmittedToAdmin = 2,
        ConfirmedByAdmin = 3,
        RejectedByAdmin = 4
    }

    public class OrderSellerFulfillment
    {
        public int Id { get; set; }

        public int OrderId { get; set; }
        public Order Order { get; set; } = null!;

        public string? SellerId { get; set; }
        public AppUser? Seller { get; set; }

        public SellerFulfillmentStatus Status { get; set; } = SellerFulfillmentStatus.Pending;

        public DateTime? SubmittedAt { get; set; }

        public DateTime? ReviewedAt { get; set; }

        public string? DecisionReason { get; set; }
    }
}
