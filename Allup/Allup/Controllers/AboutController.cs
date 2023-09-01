using Microsoft.AspNetCore.Mvc;

namespace Allup.Controllers
{
    public class AboutController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
