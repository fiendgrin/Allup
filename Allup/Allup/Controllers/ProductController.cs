using Allup.DataAccessLayer;
using Allup.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Allup.Controllers
{
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;

        public ProductController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Search(string search, int? categoryId)
        {
            List<Product> products = null;
            if (categoryId != null && await _context.Categories.AnyAsync(c => c.IsDeleted == false && c.Id == categoryId))
            {
                products = await _context.Products
                .Where(p => p.IsDeleted == false && p.CategoryId == (int)categoryId || (
                p.Title.ToLower().Contains(search.ToLower()) ||
                p.Brand != null ? p.Brand.Name.ToLower().Contains(search.ToLower()) : true)).ToListAsync();

            }
            else
            {
                products = await _context.Products
                .Where(p => p.IsDeleted == false || (
                p.Title.ToLower().Contains(search.ToLower()) ||
                p.Brand != null ? p.Brand.Name.ToLower().Contains(search.ToLower()) : true) ||
                p.Category.Name.ToLower().Contains(search.ToLower())
                ).ToListAsync();
            }

            return PartialView("_SearchPartial", products);

            //return Json(products);

        }

        public async Task<IActionResult> Modal(int? id)
        {
            if (id == null) return BadRequest("Id is required");

            Product product = await _context.Products
                .Include(p => p.ProductImages.Where(pi => pi.IsDeleted == false)).
                FirstOrDefaultAsync(p => p.IsDeleted == false && p.Id == id);

            if (product == null) return NotFound("This Id does not exist");

            return PartialView("_ModalPartial");
        }

       
    }
}
