namespace BackEnd.DTOs.Order.Customer
{
    public class OrderItemDetailsDto
    {
        public int? ProductId { get; set; }

        public string ProductName { get; set; } = string.Empty;

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal LineTotal { get; set; }
    }
}
