using Microsoft.AspNetCore.Mvc;

namespace OnlineQRMenuApp.Controllers
{
    public class ProductsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Pricing()
        {
            return View();
        }
    }
}