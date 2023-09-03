using Microsoft.AspNetCore.Mvc;

namespace Allup.Controllers
{
    public class ContactController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
