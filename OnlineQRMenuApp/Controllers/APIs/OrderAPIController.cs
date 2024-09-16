using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using OnlineQRMenuApp.Dto;
using OnlineQRMenuApp.Hubs;
using OnlineQRMenuApp.Models;
using OnlineQRMenuApp.Models.ViewModel;
namespace OnlineQRMenuApp.Controllers.APIs
{
    public class OrderRequest
    {
        public int? userId { get; set; }
        public int coffeeShopId { get; set; }
        public int tableId { get; set; }
        public List<OrderDto> items { get; set; }
        public string paymentMethod { get; set; }
        public string? deviceId { get; set; }
    }

    [Route("api/order")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly OnlineCoffeeManagementContext _context;
        private readonly IHubContext<AppHub<OrderProcessDto>> _hubContext;
        private readonly AppHub<OrderProcessDto> _appHub;

        public OrdersController(OnlineCoffeeManagementContext context, IHubContext<AppHub<OrderProcessDto>> hubContext, AppHub<OrderProcessDto> appHub)
        {
            _context = context;
            _hubContext = hubContext;
            _appHub = appHub;
        }

        // GET: api/Orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders([FromQuery] int shopId)
        {
            return await _context.Orders.Where(o => o.CoffeeShopId == shopId).Include(o => o.CoffeeShop).Include(o => o.User).ToListAsync();
        }

        // GET: api/Orders/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            var order = await _context.Orders
                .Include(o => o.CoffeeShop)
                .Include(o => o.User)
                .FirstOrDefaultAsync(m => m.OrderId == id);

            if (order == null)
            {
                return NotFound();
            }

            return order;
        }

        [HttpGet("getOrderItems/{id}")]
        public async Task<ActionResult<IEnumerable<OrderItem>>> GetOrderItems(int id)
        {
            var orderItems = await _context.OrderItems
                .Where(m => m.OrderId == id).ToListAsync();

            if (orderItems == null || !orderItems.Any())
            {
                return NotFound();
            }

            return orderItems;
        }

        [HttpPost]
        public async Task<IActionResult> PostOrder(OrderRequest request)
        {
            decimal totalPrice = request.items.Sum(x => x.Price * x.Quantity);
            Order order = new Order
            {
                CoffeeShopId = request.coffeeShopId,
                UserId = request.userId,
                TableId = request.tableId,
                TotalPrice = totalPrice,
                OrderDate = DateTime.Now,
                UpdateDate = DateTime.Now,
                Status = "Ordered",
                PaymentMethod = request.paymentMethod,
                DeviceId = request.deviceId
            };
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            foreach (OrderDto item in request.items)
            {
                OrderItem orderItem = new OrderItem
                {
                    OrderId = order.OrderId,
                    MenuItemId = item.ProductId,
                    Quantity = item.Quantity,
                    Note = item.Note,
                    SizeOptions = item.SizeOptions
                };
                _context.OrderItems.Add(orderItem);
                await _context.SaveChangesAsync();
            }

            List<string> userConnectionString = _appHub.GetConnectionIdsByRoleAndDeviceId("customer", request.deviceId);
            List<string> coffeeshopConnectionString = _appHub.GetConnectionIdsByRoleAndId("coffeeshop", request.coffeeShopId);


            var allConnections = new List<string>();

            allConnections.AddRange(userConnectionString);
            allConnections.AddRange(coffeeshopConnectionString);

            _hubContext.Clients.Clients(allConnections).SendAsync("ReceiveOrderStatus", new OrderProcessDto
            {
                orderId = order.OrderId,
                status = "Ordered",
                updateDate = order.UpdateDate,
                paymentMethod = order.PaymentMethod,
                orderDate = order.OrderDate
            });

            var response = new
            {
                OrderId = order.OrderId,
                Status = "Ordered",
                UpdateDate = order.UpdateDate,
                PaymentMethod = order.PaymentMethod,
                OrderDate = order.OrderDate
            };

            return Ok(response);
        }

        [HttpPut("updateOrderStatus")]
        public async Task<IActionResult> UpdateOrderStatus(OrderProcessDto orderProcessDto)
        {
            var order = await _context.Orders.FindAsync(orderProcessDto.orderId);
            if (order == null)
            {
                return NotFound();
            }

            order.Status = orderProcessDto.status;
            order.UpdateDate = DateTime.Now;
            order.PaymentMethod = orderProcessDto.paymentMethod;
            await _context.SaveChangesAsync();

            List<string> userConnectionString = _appHub.GetConnectionIdsByRoleAndDeviceId("customer", order.DeviceId);
            List<string> coffeeshopConnectionString = _appHub.GetConnectionIdsByRoleAndId("coffeeshop", order.CoffeeShopId);

            var allConnections = new List<string>();

            allConnections.AddRange(userConnectionString);
            allConnections.AddRange(coffeeshopConnectionString);

            _hubContext.Clients.Clients(allConnections).SendAsync("ReceiveOrderStatus", new OrderProcessDto
            {
                orderId = order.OrderId,
                status = order.Status,
                updateDate = order.UpdateDate,
                paymentMethod = order.PaymentMethod,
                orderDate = order.OrderDate
            });

            return NoContent();
        }

        // PUT: api/Orders/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrder(int id, Order order)
        {
            if (id != order.OrderId)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Entry(order).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Orders/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.OrderId == id);
        }
    }
}
