using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineQRMenuApp.Models;
using System.Collections.Generic;
using System.Linq;
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
    }
}
