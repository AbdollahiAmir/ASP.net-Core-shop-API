using BackEnd.DTOs.Order.Customer;
using BackEnd.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BackEnd.Controllers.Order
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Customer")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _service;

        public OrdersController(IOrderService service)
        {
            _service = service;
        }

        private string? currentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier);

        [HttpPost]
        public async Task<IActionResult> CreateOrder(CreateOrderDto dto)
        {
            var customerId = currentUserId;
            if (string.IsNullOrEmpty(customerId))
            {
                return Unauthorized();
            }

            try
            {
                var order = await _service.CreateFromCartAsync(customerId, dto);

                return CreatedAtAction(nameof(CreateOrder), new { id = order.Id }, order);
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

        [HttpGet]
        public async Task<IActionResult> GetMyOrders()
        {
            var customerId = currentUserId;

            if (string.IsNullOrEmpty(customerId))
            {
                return Unauthorized();
            }

            var orders = await _service.GetMyOrdersAsync(customerId);

            return Ok(orders);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetMyOrderById(int id)
        {
            var customerId = currentUserId;

            if (string.IsNullOrEmpty(customerId))
            {
                return Unauthorized();
            }

            var order = await _service.GetMyOrderByIdAsync(customerId, id);

            if (order is null)
            {
                return NotFound(new { message = "سفارش یافت نشد" });
            }

            return Ok(order);
        }

        [HttpPatch("{id:int}/cancel")]
        public async Task<IActionResult> CancelMyOrder(int id)
        {
            var customerId = currentUserId;

            if (string.IsNullOrEmpty(customerId))
            {
                return Unauthorized();
            }

            try
            {
                var order = await _service.CancelMyOrderAsync(customerId, id);

                if (order is null)
                {
                    return NotFound(new
                    {
                        message = "سفارش یافت نشد"
                    });
                }
                return Ok(order);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }
    }
}
