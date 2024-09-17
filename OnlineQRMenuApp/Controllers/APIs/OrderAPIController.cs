using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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
using OnlineQRMenuApp.Service;
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
        private readonly ConnectionMappingService _connectionService;

        public OrdersController(OnlineCoffeeManagementContext context, IHubContext<AppHub<OrderProcessDto>> hubContext, ConnectionMappingService connectionService)
        {
            _context = context;
            _hubContext = hubContext;
            _connectionService = connectionService;
        }

        // GET: api/Orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized("Bạn cần đăng nhập để xem danh sách đơn hàng.");
            }

            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
            var userTypeClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);

            if (userIdClaim == null || userTypeClaim == null)
            {
                return Unauthorized("Không thể xác định userId hoặc userType.");
            }

            var userId = int.Parse(userIdClaim.Value);
            var userType = userTypeClaim.Value;

            if (userType != "CoffeeShopManager" && userType != "Admin")
            {
                return Forbid("Bạn không có quyền truy cập danh sách đơn hàng.");
            }

            var coffeeShop = await _context.CoffeeShops.FirstOrDefaultAsync(c => c.UserId == userId);
            if (coffeeShop == null)
            {
                return NotFound("Không tìm thấy thông tin cửa hàng liên kết với tài khoản này.");
            }

            var orders = await _context.Orders.Where(o => o.CoffeeShopId == coffeeShop.CoffeeShopId).Select(o => new
            {
                o.OrderId,
                o.OrderDate,
                o.TotalPrice,
                o.Status,
                o.PaymentMethod,
                o.TableId,
                ItemCount = _context.OrderItems.Count(oi => oi.OrderId == o.OrderId)
            }).OrderByDescending(o => o.OrderDate).ToListAsync();

            var groupedOrders = orders
                .GroupBy(o => o.OrderDate.Date)
                .Select(g => new OrderListViewModel
                {
                    Date = g.Key.ToString("yyyy-MM-dd"),
                    Children = g.Select(o => new OrderViewModel
                    {
                        Code = o.OrderId.ToString(),
                        Time = o.OrderDate,
                        Status = o.Status,
                        Price = o.TotalPrice.ToString("0.00"),
                        Quantity = o.ItemCount.ToString(),
                        Table = o.TableId.ToString(),
                        PaymentStatus = "Paid",
                        PaymentMethod = o.PaymentMethod
                    }).ToList()
                })
                .ToList();

            return Ok(groupedOrders);
        }

        [HttpGet("getOrderItems")]
        public async Task<ActionResult<IEnumerable<OrderItem>>> GetOrderItems(int id)
        {
            var orderItems = await _context.OrderItems.Include(oT => oT.MenuItem).Where(m => m.OrderId == id).Select(ot => new
            {
                ot.OrderItemId,
                ot.MenuItemId,
                ot.Note,
                ot.SizeOptions,
                ot.Quantity,
                Name = ot.MenuItem.Name,
                Image = ot.MenuItem.Image,
                Description = ot.MenuItem.Description,
                ItemPrice = ot.MenuItem.Price,
                Type = ot.MenuItem.Type
            }).ToListAsync();

            if (orderItems == null || !orderItems.Any())
            {
                return NotFound();
            }

            return Ok(orderItems);
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

            List<string> userConnectionIds = _connectionService.GetConnectionIdsByRoleAndDeviceId("customer", request.deviceId);
            List<string> coffeeshopConnectionIds = _connectionService.GetConnectionIdsByRoleAndId("coffeeshop", request.coffeeShopId);

            var allConnections = new List<string>();

            allConnections.AddRange(userConnectionIds);
            allConnections.AddRange(coffeeshopConnectionIds);

            if (allConnections.Any())
            {
                _hubContext.Clients.Clients(allConnections).SendAsync("ReceiveOrderStatus", new OrderProcessDto
                {
                    orderId = order.OrderId,
                    status = order.Status,
                    updateDate = order.UpdateDate,
                    paymentMethod = order.PaymentMethod,
                    orderDate = order.OrderDate
                });
            }

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

            List<string> userConnectionIds = _connectionService.GetConnectionIdsByRoleAndDeviceId("customer", order.DeviceId);
            List<string> coffeeshopConnectionIds = _connectionService.GetConnectionIdsByRoleAndId("coffeeshop", order.CoffeeShopId);

            var allConnections = new List<string>();
            allConnections.AddRange(userConnectionIds);
            allConnections.AddRange(coffeeshopConnectionIds);
            if (allConnections.Any())
            {
                _hubContext.Clients.Clients(allConnections).SendAsync("ReceiveOrderStatus", new OrderProcessDto
                {
                    orderId = order.OrderId,
                    status = order.Status,
                    updateDate = order.UpdateDate,
                    paymentMethod = order.PaymentMethod,
                    orderDate = order.OrderDate
                });
            }

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
