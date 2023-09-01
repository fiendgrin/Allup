using Allup.DataAccessLayer;
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
            HomeVM homeVM = new HomeVM();

            homeVM.Slider = await _context.Sliders.Where(slide => slide.IsDeleted == false).ToListAsync();
            
            return View(homeVM);
        }
    }
}
