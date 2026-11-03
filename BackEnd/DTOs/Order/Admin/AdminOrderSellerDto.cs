namespace BackEnd.DTOs.Order.Admin
{
    public class AdminOrderSellerDto
    {
        public string SellerUserName { get; set; } = string.Empty;
        public string FulfillmentStatus { get; set; } = string.Empty;
        public DateTime? SubmittedAt { get; set; }
        public DateTime? ReviewedAt { get; set; }
        public string? DecisionReason { get; set; }

        public string SellerId { get; set; } = string.Empty;
    }
}
