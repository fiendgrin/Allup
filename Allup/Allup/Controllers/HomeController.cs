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
            HomeVM HomeVM = new HomeVM();

            HomeVM.Sliders = await _context.Sliders.Where(s => s.IsDeleted == false).ToListAsync();
            HomeVM.Categories = await _context.Categories.Where(c => c.IsDeleted == false && c.IsMain).ToListAsync();
            HomeVM.NewArrival = await _context.Products.Where(s => s.IsDeleted == false && s.IsNewArrival).ToListAsync();
            HomeVM.BestSeller = await _context.Products.Where(s => s.IsDeleted == false && s.IsBestSeller).ToListAsync();
            HomeVM.Featured = await _context.Products.Where(s => s.IsDeleted == false && s.IsFeatured).ToListAsync();



            return View(HomeVM);
        }

        public IActionResult GetCookies()
        {
            string cookie = Request.Cookies["basket"];

            return Ok(cookie);
        }
    }
}
