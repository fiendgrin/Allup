using Microsoft.AspNetCore.Mvc;

namespace Allup.Controllers
{
    public class ProductController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
