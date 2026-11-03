namespace BackEnd.DTOs.Order.Customer
{
    public class OrderDetailsDto
    {
        public int Id { get; set; }

        public DateTime CreatedAt { get; set; }

        public string DeliveryAddress { get; set; } = string.Empty;

        public decimal TotalAmount { get; set; }

        public string Status { get; set; } = string.Empty;

        public string? DecisionReason { get; set; }

        public DateTime? FinalizedAt { get; set; }

        public List<OrderItemDetailsDto> Items { get; set; } = new();
    }
}
