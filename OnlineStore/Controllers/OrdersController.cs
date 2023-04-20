using Microsoft.AspNetCore.Mvc;
using OrderManagement.Enums;
using Models.Orders;
using OrderManagement.Services;

namespace OrderManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly OrdersService ordersService;
        public OrdersController(OrdersService ordersService)
        {
            this.ordersService = ordersService;
        }

        [HttpPost("Create")]
        public async Task<string> CreateOrder(OrderStoreParams order)
        {
            return await this.ordersService.CreateOrder(order);
        }

        [HttpGet("{id}")]
        public async Task<Order> GetOrder(string id)
        {
            return await this.ordersService.GetOrder(id);
        }

        [HttpPut("Status")]
        public ActionResult UpdateOrderStatus(OrderStatus status)
        {
            return Ok("Order status is successfully updated");
        }
    }
}