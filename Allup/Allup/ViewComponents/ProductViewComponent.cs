using Allup.DataAccessLayer;
using Allup.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Allup.ViewComponents
{
    public class ProductViewComponent : ViewComponent
    {
        private readonly AppDbContext _context;

        public ProductViewComponent(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync() 
        {
            HomeVM Productvm = new HomeVM();
            Productvm.NewArrival = _context.Products.Where(p => p.IsDeleted == false && p.IsNewArrival == true);
            Productvm.BestSeller = _context.Products.Where(p => p.IsDeleted == false && p.IsBestSeller == true);
            Productvm.Featured = _context.Products.Where(p => p.IsDeleted == false && p.IsFeatured == true);

            return View(Productvm);
        }
    }
}
