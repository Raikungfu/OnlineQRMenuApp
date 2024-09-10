using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineQRMenuApp.Models;

namespace OnlineQRMenuApp.Controllers.APIs
{
    [Route("api/CoffeeShops")]
    [ApiController]
    public class CoffeeShopAPIController : ControllerBase
    {
        private readonly OnlineCoffeeManagementContext _context;

        public CoffeeShopAPIController(OnlineCoffeeManagementContext context)
        {
            _context = context;
        }

        // GET: api/CoffeeShops/Details/5
        [HttpGet("Details/{id}")]
        public async Task<ActionResult<CoffeeShop>> GetCoffeeShop(int id)
        {
            var coffeeShop = await _context.CoffeeShops
                .Where(m => m.CoffeeShopId == id)
                .Select(m => new CoffeeShop
                {
                    CoffeeShopId = m.CoffeeShopId,
                    Name = m.Name,
                    Location = m.Location,
                    PrimaryColor = m.PrimaryColor,
                    SecondaryColor = m.SecondaryColor,
                    Description = m.Description,
                    Slogan = m.Slogan,
                    Avatar = m.Avatar,
                    CoverImage = m.CoverImage,
                    Hotline = m.Hotline,
                    Email = m.Email,
                    Website = m.Website,
                    Facebook = m.Facebook,
                    Instagram = m.Instagram,
                    Twitter = m.Twitter,
                    OpeningHours = m.OpeningHours
                })
                .FirstOrDefaultAsync();

            if (coffeeShop == null)
            {
                return NotFound();
            }

            return coffeeShop;
        }


    }
}
