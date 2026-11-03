namespace BackEnd.DTOs.Order.Admin
{
    public class AdminOrderDto
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }

        public string CustomerUserName { get; set; } = string.Empty;
        public string DeliveryAddress { get; set; } = string.Empty;

        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? DecisionReason { get; set; }
        public DateTime? FinalizedAt { get; set; }

        public List<AdminOrderItemDto> Items { get; set; } = new();
        public List<AdminOrderSellerDto> Sellers { get; set; } = new();
    }
}
