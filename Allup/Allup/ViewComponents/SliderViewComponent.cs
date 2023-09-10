using Allup.DataAccessLayer;
using Allup.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Allup.ViewComponents
{
    public class SliderViewComponent : ViewComponent
    {
        private readonly AppDbContext _context;

        public SliderViewComponent(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync() 
        {
            HomeVM homeVM = new HomeVM();

            homeVM.Sliders = await _context.Sliders.Where(s => s.IsDeleted == false).ToListAsync();

            return View(homeVM);
        }
    }
}
