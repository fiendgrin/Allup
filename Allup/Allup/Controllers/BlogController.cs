using Microsoft.AspNetCore.Mvc;

namespace Allup.Controllers
{
    public class BlogController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
