using Microsoft.AspNetCore.Mvc;

namespace OnlineQRMenuApp.Controllers
{
    public class ProductsController : BaseController
    {

        public ProductsController(IWebHostEnvironment env, IConfiguration configuration) : base(env, configuration)
        {
        }

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