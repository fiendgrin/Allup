using Allup.DataAccessLayer;
using Allup.Services;
using Allup.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Allup.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }

        public IActionResult GetCookies()
        {
            string cookie = Request.Cookies["basket"];

            return Ok(cookie);
        }
    }
}
