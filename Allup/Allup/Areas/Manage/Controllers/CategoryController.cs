using Allup.DataAccessLayer;
using Allup.Models;
using Allup.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Allup.Areas.Manage.Controllers
{
    [Area("manage")]
    public class CategoryController : Controller
    {
        private readonly AppDbContext _context;

        public CategoryController(AppDbContext context)
        {
            _context = context;
        }
        //1.Index
        //2.Detail

        //=============================================================

        //1.Index
        public IActionResult Index(int currentPage = 1)
        {
            IQueryable<Category> queries = _context.Categories
                .Include(c => c.Children.Where(ch => ch.IsDeleted == false))
                .Include(c => c.Products.Where(p => p.IsDeleted == false))
                .Where(c => c.IsDeleted == false && c.IsMain == true);
            return View(PageNatedList<Category>.Create(queries, currentPage, 5, 6));
        }

        //2.Detail
        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null) return BadRequest();
            Category category = await _context.Categories
                .Include(c => c.Children.Where(ch => ch.IsDeleted == false)).ThenInclude(c => c.Products.Where(p => p.IsDeleted == false))
                .Include(c => c.Products.Where(p => p.IsDeleted == false))
                .FirstOrDefaultAsync(c => c.IsDeleted == false && c.Id == id);

            if (category == null) return NotFound();


            return View(category);
        }

        public async Task<IActionResult> Create()
        {
            List<Category> categories = await _context.Categories
                .Where(c => c.IsDeleted == false && c.IsMain == true)
                .ToListAsync();

            ViewBag.Categories = categories;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Category category)
        {
            List<Category> categories = await _context.Categories
                .Where(c => c.IsDeleted == false && c.IsMain == true)
                .ToListAsync();

            ViewBag.Categories = categories;

            if (!ModelState.IsValid) return View(category);

            if (category.IsMain)
            {
                if (string.IsNullOrWhiteSpace(category.Image))
                {
                    ModelState.AddModelError("Image", "Required");
                    return View(category);
                }

                category.ParentId = null;
            }
            else
            {
                if (category.ParentId == null)
                {
                    ModelState.AddModelError("ParentId", "Required");
                    return View(category);
                }

                if (!await _context.Categories.AnyAsync(c => c.IsDeleted == false && c.IsMain == true && c.Id == category.ParentId))
                {
                    ModelState.AddModelError("ParentId", "Is Incorrect");
                    return View(category);
                }
                category.Image = null;
            }

            if (await _context.Categories.AnyAsync(c=>c.IsDeleted == false && c.Name.ToLower() == category.Name.Trim().ToLower()))
            {
                ModelState.AddModelError("Name", "Already Exists");
                return View(category);
            }

            category.Name = category.Name.Trim();

            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
