using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Allup.Areas.Manage.Controllers
{
    public class DashboardController : Controller
    {
        [Area("Manage")]
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }
    }
}
