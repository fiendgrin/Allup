using Allup.DataAccessLayer;
using Allup.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Allup.ViewComponents
{
    public class CategoryViewComponent : ViewComponent
    {
        private readonly AppDbContext _context;

        public CategoryViewComponent(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync() 
        {
            HomeVM categoryVM = new HomeVM();
            categoryVM.Categories = await _context.Categories.Where(c => c.IsDeleted == false && c.IsMain).ToListAsync();
            return View(categoryVM);
        }
    }
}
