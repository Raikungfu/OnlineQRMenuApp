using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using OnlineQRMenuApp.Dto;
using OnlineQRMenuApp.Hubs;
using OnlineQRMenuApp.Models;
using OnlineQRMenuApp.Models.ViewModel;
using OnlineQRMenuApp.Service;
using System.Linq;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace OnlineQRMenuApp.Controllers.APIs
{
    public enum StatisticType
    {
        Daily,
        Monthly,
        Yearly
    }

    [Route("api/statistic")]
    [ApiController]
    public class StatisticAPIController : ControllerBase
    {
        private readonly OnlineCoffeeManagementContext _context;
        private readonly IHubContext<AppHub<OrderProcessDto>> _hubContext;

        public StatisticAPIController(OnlineCoffeeManagementContext context, IHubContext<AppHub<OrderProcessDto>> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        [HttpGet]
        public async Task<IActionResult> Statistic([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate, [FromQuery] StatisticType? statisticType = StatisticType.Daily)
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

            if (userType != "CoffeeShopManager")
            {
                return Forbid("Bạn không có quyền truy cập danh sách đơn hàng.");
            }

            if (!startDate.HasValue || !endDate.HasValue)
            {
                var today = DateTime.Today;
                startDate = new DateTime(today.Year, today.Month, 1);
                endDate = startDate.Value.AddMonths(1).AddDays(-1);
            }

            var metrics = await GetMetricsAsync(startDate, endDate, statisticType, userId);
            return Ok(metrics);
        }

        public async Task<ChartData> GetMetricsAsync(DateTime? startDate, DateTime? endDate, StatisticType? statisticType, int userId)
        {
            IQueryable<dynamic> query = null;

            switch (statisticType)
            {
                case StatisticType.Daily:
                    query = _context.OrderItems
                        .Join(_context.Orders,
                              oi => oi.OrderId,
                              o => o.OrderId,
                              (oi, o) => new { oi, o })
                        .Where(x => x.o.OrderDate >= startDate && x.o.OrderDate <= endDate && x.o.CoffeeShopId == userId)
                        .GroupBy(x => x.o.OrderDate.Date)
                        .Select(g => new
                        {
                            X = g.Key.ToString("dd/MM/yyyy"),
                            Revenue = g.Sum(x => x.oi.Price * x.oi.Quantity),
                            Profit = g.Sum(x => (x.oi.Price - x.oi.Cost) * x.oi.Quantity),
                            Cost = g.Sum(x => x.oi.Cost * x.oi.Quantity),
                            Sales = g.Sum(x => x.oi.Quantity)
                        });
                    break;

                case StatisticType.Monthly:
                    query = _context.OrderItems
                        .Join(_context.Orders,
                              oi => oi.OrderId,
                              o => o.OrderId,
                              (oi, o) => new { oi, o })
                        .Where(x => x.o.OrderDate >= startDate && x.o.OrderDate <= endDate && x.o.CoffeeShopId == userId)
                        .GroupBy(x => new { Year = x.o.OrderDate.Year, Month = x.o.OrderDate.Month })
                        .Select(g => new
                        {
                            X = $"{g.Key.Month}/{g.Key.Year}",
                            Revenue = g.Sum(x => x.oi.Price * x.oi.Quantity),
                            Profit = g.Sum(x => (x.oi.Price - x.oi.Cost) * x.oi.Quantity),
                            Cost = g.Sum(x => x.oi.Cost * x.oi.Quantity),
                            Sales = g.Sum(x => x.oi.Quantity)
                        });
                    break;

                case StatisticType.Yearly:
                    query = _context.OrderItems
                        .Join(_context.Orders,
                              oi => oi.OrderId,
                              o => o.OrderId,
                              (oi, o) => new { oi, o })
                        .Where(x => x.o.OrderDate >= startDate && x.o.OrderDate <= endDate && x.o.CoffeeShopId == userId)
                        .GroupBy(x => x.o.OrderDate.Year)
                        .Select(g => new
                        {
                            X = g.Key.ToString(),
                            Revenue = g.Sum(x => x.oi.Price * x.oi.Quantity),
                            Profit = g.Sum(x => (x.oi.Price - x.oi.Cost) * x.oi.Quantity),
                            Cost = g.Sum(x => x.oi.Cost * x.oi.Quantity),
                            Sales = g.Sum(x => x.oi.Quantity)
                        });
                    break;
            }

            if (query == null)
            {
                throw new InvalidOperationException("Query không được khởi tạo.");
            }

            var data = await query.AsNoTracking().ToListAsync();

            var totalRevenue = data.Sum(d => (decimal) d.Revenue);
            var totalProfit = data.Sum(d => (decimal) d.Profit);
            var totalCost = data.Sum(d => (decimal) d.Cost);
            var totalSales = data.Sum(d => (decimal) d.Sales);

            var chartData = new ChartData
            {
                DataStatistic = new List<DataStatistic>
{
    new DataStatistic
    {
        id = "Doanh thu",
        data = data.Select(d => new DataPoint { x = d.X, y = d.Revenue }).ToList(),
    },
    new DataStatistic
    {
        id = "Lợi nhuận",
        data = data.Select(d => new DataPoint { x = d.X, y = d.Profit }).ToList(),
    },
    new DataStatistic
    {
        id = "Chi phí",
        data = data.Select(d => new DataPoint { x = d.X, y = d.Cost }).ToList(),
    },
    new DataStatistic
    {
        id = "Đã bán",
        data = data.Select(d => new DataPoint { x = d.X, y = d.Sales }).ToList(),
    }
},
                TotalRevenue = totalRevenue,
                TotalProfit = totalProfit,
                TotalCost = totalCost,
                TotalSales = totalSales
            };

            return chartData;
        }


    }
}
