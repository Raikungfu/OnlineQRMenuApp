using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using OnlineQRMenuApp.Dto;
using OnlineQRMenuApp.Hubs;
using OnlineQRMenuApp.Models;
namespace OnlineQRMenuApp.Controllers.APIs
{
    public class OrderRequest
    {
        public int? userId { get; set; }
        public int coffeeShopId { get; set; }
        public int tableId { get; set; }
        public List<OrderDto> items { get; set; }
        public string paymentMethod { get; set; }
    }

    [Route("api/order")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly OnlineCoffeeManagementContext _context;
        private readonly IHubContext<AppHub<OrderProcessDto>> _hubContext;

        public OrdersController(OnlineCoffeeManagementContext context, IHubContext<AppHub<OrderProcessDto>> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        // GET: api/Orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            return await _context.Orders.Include(o => o.CoffeeShop).Include(o => o.User).ToListAsync();
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
                Status = "Confirm",
                PaymentMethod = request.paymentMethod
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

            _hubContext.Clients.All.SendAsync("ReceiveOrderStatus", new OrderProcessDto
            {
                orderId = order.OrderId,
                status = "Confirm",
                updateDate = order.UpdateDate,
                paymentMethod = order.PaymentMethod,
                orderDate = order.OrderDate
            });

            var response = new
            {
                OrderId = order.OrderId,
                Status = "Confirm",
                UpdateDate = order.UpdateDate,
                PaymentMethod = order.PaymentMethod,
                OrderDate = order.OrderDate
            };

            return Ok(response);
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
