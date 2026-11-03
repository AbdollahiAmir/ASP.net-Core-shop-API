namespace BackEnd.DTOs.Order.Admin
{
    public class AdminOrderItemDto
    {
        public string ProductName { get; set; } = string.Empty;
        public string SellerUserName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal { get; set; }
    }
}
