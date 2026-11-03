using BackEnd.DTOs.Order.Admin;
using BackEnd.DTOs.Order.Customer;
using BackEnd.DTOs.Order.Seller;

namespace BackEnd.IServices
{
    public interface IOrderService
    {
        // Customer

        Task<OrderDto> CreateFromCartAsync(string customerId, CreateOrderDto dto);

        Task<List<OrderDetailsDto>> GetMyOrdersAsync(string customerId);

        Task<OrderDetailsDto?> GetMyOrderByIdAsync(string customerId, int orderId);

        Task<OrderDetailsDto?> CancelMyOrderAsync(string customerId, int orderId);

        // Seller

        Task<List<SellerOrderDto>> GetSellerOrdersAsync(string sellerId);

        Task<SellerOrderDto?> GetSellerOrderByIdAsync(string sellerId, int orderId);

        Task<SellerOrderDto?> WithdrawSellerSubmissionAsync(string sellerId, int orderId);

        Task<SellerOrderDto?> UpdateSellerFulfillmentAsync(string sellerId, int orderId, SellerFulfillmentDecisionDto dto);

        // Admin 

        Task<List<AdminOrderDto>> GetAllOrdersForAdminAsync();

        Task<AdminOrderDto?> GetOrderForAdminByIdAsync(int orderId);

        Task<AdminOrderDto?> ReviewSellerFulfillmentAsync(int orderId, string sellerId, AdminSellerDecisionDto dto);

        Task<AdminOrderDto?> FinalApproveOrderAsync(int orderId);

        Task<AdminOrderDto?> RejectOrderAsync(int orderId, AdminOrderDecisionDto dto);

        Task<AdminOrderDto?> WithdrawSellerApprovalAsync(int orderId, string sellerId);

        Task<AdminOrderDto?> CancelOrderByAdminAsync(int orderId, AdminOrderDecisionDto dto);

        Task<bool> DeleteOrderAsync(int orderId);
    }
}