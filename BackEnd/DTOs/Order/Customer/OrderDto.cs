namespace BackEnd.DTOs.Order.Customer
{
    public class OrderDto
    {
        public int Id { get; set; }

        public DateTime CreatedAt { get; set; }

        public string DeliveryAddress { get; set; } = string.Empty;

        public decimal TotalAmount { get; set; }

        public string Status { get; set; } = string.Empty;

        public int ItemCount { get; set; }
    }
}
