using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineQRMenuApp.Models;
using OnlineQRMenuApp.Models.ViewModel;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OnlineQRMenuApp.Controllers.APIs
{
    [Route("api/menu")]
    [ApiController]
    public class MenuItemsApiController : ControllerBase
    {
        private readonly OnlineCoffeeManagementContext _context;
        private readonly IMapper _mapper;

        public MenuItemsApiController(OnlineCoffeeManagementContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/MenuItems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MenuItem>>> GetMenuItems()
        {
            return await _context.MenuItems.Include(m => m.Category).ToListAsync();
        }

        // GET: api/MenuItems/5
        [HttpGet("item")]
        public async Task<ActionResult<MenuItemsModel>> GetMenuItem([FromQuery] int shopId, [FromQuery] int productId)
        {
            var menuItem = await _context.MenuItems.Where(m => m.Category.CoffeeShopId == shopId)
                .Include(m => m.CustomizationGroups).ThenInclude(mC => mC.Customizations).AsNoTracking()
                .FirstOrDefaultAsync(m => m.MenuItemId == productId);

            if (menuItem == null)
            {
                return NotFound();
            }
            var menuItemsModel = _mapper.Map<MenuItemsModel>(menuItem);

            return menuItemsModel;
        }

        [HttpGet("customize-option")]
        public async Task<ActionResult<List<CustomizationGroup>>> GetMenuItemCustomizeOption([FromQuery] int productId)
        {
            var menuItem = await _context.CustomizationGroups.Include(mC => mC.Customizations).Where(mC => mC.MenuItemId == productId).ToArrayAsync();

            if (menuItem == null)
            {
                return NotFound();
            }
            var menuItemsModel = _mapper.Map<List<CustomizationGroup>>(menuItem);

            return menuItemsModel;
        }

        [HttpGet("product")]
        public async Task<ActionResult<IEnumerable<MenuItem>>> GetMenu([FromQuery] int shopId, [FromQuery] int? categoryId)
        {
            var query = _context.Categories
                .Include(c => c.MenuItems)
                .Where(c => c.CoffeeShopId == shopId);

            if (categoryId.HasValue && categoryId > 0)
            {
                query = query.Where(c => c.CategoryId == categoryId);
            }

            var categories = await query.AsNoTracking().ToListAsync();

            var menuItems = categories.SelectMany(c => c.MenuItems).ToList();

            if (menuItems == null || !menuItems.Any())
            {
                return NotFound();
            }

            return Ok(menuItems);
        }

        [HttpGet("product-coffeeshop")]
        public async Task<ActionResult<IEnumerable<MenuItem>>> GetMenuItem([FromQuery] int? categoryId)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized("Bạn cần đăng nhập để xem danh sách đơn hàng.");
            }

            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
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

            var query = _context.Categories
                .Include(c => c.MenuItems)
                .Where(c => c.CoffeeShopId == coffeeShop.CoffeeShopId);

            if (categoryId.HasValue && categoryId > 0)
            {
                query = query.Where(c => c.CategoryId == categoryId);
            }

            var categories = await query.AsNoTracking().ToListAsync();

            var menuItems = categories.SelectMany(c => c.MenuItems).ToList();

            if (menuItems == null || !menuItems.Any())
            {
                return NotFound();
            }

            return Ok(menuItems);
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<MenuItem>>> GetMenuSearch([FromQuery] int shopId, [FromQuery] string? searchQuery, [FromQuery] int? categoryId)
        {
            var normalizedSearchQuery = string.IsNullOrWhiteSpace(searchQuery) ? string.Empty : searchQuery.Trim().ToLower();

            var query = _context.MenuItems
                .Where(mi => mi.Category.CoffeeShopId == shopId);

            if (categoryId.HasValue && categoryId > 0)
            {
                query = query.Where(mi => mi.CategoryId == categoryId);
            }

            if (!string.IsNullOrEmpty(normalizedSearchQuery))
            {
                query = query.Where(mi => mi.Name.ToLower().Contains(normalizedSearchQuery));
            }

            var menuItems = await query.AsNoTracking().ToListAsync();

            if (menuItems == null || !menuItems.Any())
            {
                return NotFound();
            }

            return Ok(menuItems);
        }

        // POST: api/MenuItems
        [HttpPost]
        public async Task<ActionResult<MenuItem>> PostMenuItem(MenuItem menuItem)
        {
            _context.MenuItems.Add(menuItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMenuItem", new { id = menuItem.MenuItemId }, menuItem);
        }

        // PUT: api/MenuItems/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMenuItem(int id, MenuItem menuItem)
        {
            if (id != menuItem.MenuItemId)
            {
                return BadRequest();
            }

            _context.Entry(menuItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MenuItemExists(id))
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

        // DELETE: api/MenuItems/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMenuItem(int id)
        {
            var menuItem = await _context.MenuItems.FindAsync(id);
            if (menuItem == null)
            {
                return NotFound();
            }

            _context.MenuItems.Remove(menuItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MenuItemExists(int id)
        {
            return _context.MenuItems.Any(e => e.MenuItemId == id);
        }
    }
}
