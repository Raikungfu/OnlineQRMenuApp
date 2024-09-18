using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineQRMenuApp.Models;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OnlineQRMenuApp.Controllers.APIs
{
    [Route("api/Categories")]
    [ApiController]
    public class CategoriesAPIController : ControllerBase
    {
        private readonly OnlineCoffeeManagementContext _context;

        public CategoriesAPIController(OnlineCoffeeManagementContext context)
        {
            _context = context;
        }

        // GET: api/Categories/5
        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategories(int id)
        {
            var categories = await _context.Categories
                .Where(m => m.CoffeeShopId == id).ToListAsync();

            if (categories == null || !categories.Any())
            {
                return NotFound();
            }

            return categories;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategoriesCoffeeShop()
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
            var categories = await _context.Categories
                .Where(m => m.CoffeeShopId == coffeeShop.CoffeeShopId).ToListAsync();

            if (categories == null || !categories.Any())
            {
                return NotFound();
            }

            return categories;
        }
    }
}
