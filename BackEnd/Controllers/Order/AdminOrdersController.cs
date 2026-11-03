using BackEnd.DTOs.Order.Admin;
using BackEnd.IServices;
using BackEnd.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Controllers.Order
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminOrdersController : ControllerBase
    {
        private readonly IOrderService _service;

        public AdminOrdersController(IOrderService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _service.GetAllOrdersForAdminAsync();
            return Ok(orders);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var order = await _service.GetOrderForAdminByIdAsync(id);

            if (order is null)
            {
                return NotFound(new { message = "سفارش یافت نشد" });
            }

            return Ok(order);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var deleted = await _service.DeleteOrderAsync(id);

            if (!deleted)
            {
                return NotFound(new { message = "سفارش یافت نشد" });
            }

            return Ok(new { message = "سفارش با موفقیت حذف شد" });
        }

        [HttpPatch("{orderId:int}/sellers/{sellerId}/decision")]
        public async Task<IActionResult> ReviewSellerFulfillment(int orderId, string sellerId, [FromBody] AdminSellerDecisionDto dto)
        {
            try
            {
                var order = await _service.ReviewSellerFulfillmentAsync(orderId, sellerId, dto);

                if (order is null)
                {
                    return NotFound(new
                    {
                        message = "پاسخ فروشنده یافت نشد"
                    });
                }

                return Ok(order);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPatch("{id:int}/final-approve")]
        public async Task<IActionResult> FinalApproveOrder(int id)
        {
            try
            {
                var order = await _service.FinalApproveOrderAsync(id);

                if (order is null)
                {
                    return NotFound(new { message = "سفارش یافت نشد" });
                }

                return Ok(order);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPatch("{id:int}/reject")]
        public async Task<IActionResult> RejectOrder(int id, [FromBody] AdminOrderDecisionDto dto)
        {
            try
            {
                var order = await _service.RejectOrderAsync(id, dto);

                if (order is null)
                {
                    return NotFound(new { message = "سفارش یافت نشد" });
                }

                return Ok(order);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPatch("{id:int}/cancel")]
        public async Task<IActionResult> CancelOrderByAdmin(int id, [FromBody] AdminOrderDecisionDto dto)
        {
            try
            {
                var order = await _service.CancelOrderByAdminAsync(id, dto);

                if (order is null)
                {
                    return NotFound(new { message = "سفارش یافت نشد" });
                }

                return Ok(order);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPatch("{orderId}/sellers/{sellerId}/withdraw-approval")]
        public async Task<IActionResult> WithdrawSellerApproval(int orderId, string sellerId)
        {
            try
            {
                var result = await _service.WithdrawSellerApprovalAsync(orderId, sellerId);

                return result is null ? NotFound() : Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }


    }
}
