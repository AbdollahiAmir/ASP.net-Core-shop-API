using BackEnd.DTOs.Order.Seller;
using BackEnd.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BackEnd.Controllers.Order
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Seller,Admin")]
    public class SellerOrdersController : ControllerBase
    {
        private readonly IOrderService _service;

        public SellerOrdersController(IOrderService service)
        {
            _service = service;
        }

        private string? currentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier);

        [HttpGet]
        public async Task<IActionResult> GetMyOrders()
        {
            if (string.IsNullOrEmpty(currentUserId))
            {
                return Unauthorized();
            }

            var orders = await _service.GetSellerOrdersAsync(currentUserId);

            return Ok(orders);
        }


        [HttpGet("{orderId:int}")]
        public async Task<IActionResult> GetMyOrderById(int orderId)
        {
            if (string.IsNullOrEmpty(currentUserId))
            {
                return Unauthorized();
            }

            var order = await _service.GetSellerOrderByIdAsync(currentUserId, orderId);

            if (order is null)
            {
                return NotFound(new { message = "سفارش یافت نشد" });
            }

            return Ok(order);
        }

        [HttpPatch("{orderId:int}/fulfillment")]
        public async Task<IActionResult> UpdateFulfillment(int orderId, [FromBody] SellerFulfillmentDecisionDto dto)
        {
            var sellerId = currentUserId;

            if (string.IsNullOrEmpty(sellerId))
            {
                return Unauthorized();
            }

            try
            {
                var result = await _service.UpdateSellerFulfillmentAsync(sellerId, orderId, dto);

                if (result is null)
                {
                    return NotFound(new { message = "سفارش یافت نشد" });
                }

                return Ok(result);
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

        [HttpPatch("{orderId}/withdraw-submission")]
        public async Task<IActionResult> WithdrawSubmission(int orderId)
        {
            var sellerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(sellerId))
            {
                return Unauthorized();
            }

            try
            {
                var result = await _service.WithdrawSellerSubmissionAsync(sellerId, orderId);

                return result is null ? NotFound() : Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }
    }
}
