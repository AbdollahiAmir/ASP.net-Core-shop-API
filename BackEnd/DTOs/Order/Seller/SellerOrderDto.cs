namespace BackEnd.DTOs.Order.Seller
{
    public class SellerOrderDto
    {
        public int OrderId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; } = string.Empty;

        public List<SellerOrderItemDto> Items { get; set; } = new();

        public string FulfillmentStatus { get; set; } = string.Empty;
        public DateTime? SubmittedAt { get; set; }
        public DateTime? ReviewedAt { get; set; }
        public string? DecisionReason { get; set; }
    }
}
