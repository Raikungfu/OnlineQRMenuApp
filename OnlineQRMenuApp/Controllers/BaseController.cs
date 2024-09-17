using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace OnlineQRMenuApp.Controllers
{
    public class BaseController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _configuration;

        public BaseController(IWebHostEnvironment env, IConfiguration configuration)
        {
            _env = env;
            _configuration = configuration;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var frontendLink = _env.IsDevelopment()
                ? _configuration["DashboardLink:Development:CoffeeShop"]
                : _configuration["DashboardLink:Production:CoffeeShop"];

            ViewData["DashboardLink"] = frontendLink;

            base.OnActionExecuting(context);
        }
    }
}