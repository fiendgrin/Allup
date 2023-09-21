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

        private readonly IWebHostEnvironment _env;

        public CategoryController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        //1.Index
        //2.Detail
        //3.Create(Get)
        //4.Create(Post)
        //5.Update(Get)
        //6.Update(Post)
        //7.Delete(Get)
        //8.DeleteCategory

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
        //3.Create(Get)
        public async Task<IActionResult> Create()
        {
            List<Category> categories = await _context.Categories
                .Where(c => c.IsDeleted == false && c.IsMain == true)
                .ToListAsync();

            ViewBag.Categories = categories;

            return View();
        }
        //4.Create(Post)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            List<Category> categories = await _context.Categories
                .Where(c => c.IsDeleted == false && c.IsMain == true)
                .ToListAsync();

            ViewBag.Categories = categories;

            if (!ModelState.IsValid) return View(category);

            if (category.IsMain)
            {
                if (category.File == null)
                {
                    ModelState.AddModelError("File", "File is required");
                    return View(category);
                }

                if (category.File.ContentType != "image/jpeg")
                {
                    ModelState.AddModelError("File", "File type should be .jpg or .jpeg");
                    return View(category);
                }

                if ((category.File.Length / 1024) > 30)
                {
                    ModelState.AddModelError("File", "File can't be larger than 30kb");
                    return View(category);
                }

                string fileName = DateTime.UtcNow.AddHours(4).ToString("yyyyMMddHHmmssfff") + category.File.FileName.Substring(category.File.FileName.LastIndexOf("."));

                string filePath = Path.Combine(_env.WebRootPath, "assets", "images", fileName);

                using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await category.File.CopyToAsync(fileStream);
                }

                category.Image = fileName;

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

            if (await _context.Categories.AnyAsync(c => c.IsDeleted == false && c.Name.ToLower() == category.Name.Trim().ToLower()))
            {
                ModelState.AddModelError("Name", "Already Exists");
                return View(category);
            }

            category.Name = category.Name.Trim();

            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        //5.Update(Get)
        public async Task<IActionResult> Update(int? id)
        {
            if (id == null) return BadRequest();

            Category category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id && c.IsDeleted == false);

            if (category == null) return BadRequest();

            ViewBag.Categories = await _context.Categories
                .Where(c => c.IsDeleted == false && c.IsMain == true)
                .ToListAsync();

            return View(category);
        }
        //6.Update(Post)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id, Category category)
        {
            ViewBag.Categories = await _context.Categories
                .Where(c => c.IsDeleted == false && c.IsMain == true)
                .ToListAsync();

            if (id == null) return BadRequest();

            if (id != category.Id) return BadRequest();

            if (!ModelState.IsValid) return View(category);

            Category dbCategory = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == category.Id && c.IsDeleted == false);

            if (dbCategory == null) return BadRequest();

            if (await _context.Categories.AnyAsync(c => c.IsDeleted == false && c.Name.ToLower() == category.Name.Trim().ToLower() && c.Id != category.Id))
            {
                ModelState.AddModelError("Name", "Already Exists");
                return View(category);
            }

            if (dbCategory.IsMain != category.IsMain)
            {
                ModelState.AddModelError("IsMain", "Can not be changed");

                return View(dbCategory);
            }

            if (dbCategory.IsMain)
            {
                if (category.File != null)
                {
                    if (!category.File.ContentType.Contains("image/") )
                    {
                        ModelState.AddModelError("File", "File type should be .jpg or .jpeg");
                        return View(category);
                    }

                    if ((category.File.Length / 1024) > 30)
                    {
                        ModelState.AddModelError("File", "File can't be larger than 30kb");
                        return View(category);
                    }

                    string fileName = DateTime.UtcNow.AddHours(4).ToString("yyyyMMddHHmmssfff") + category.File.FileName.Substring(category.File.FileName.LastIndexOf("."));


                    string filePath = Path.Combine(_env.WebRootPath, "assets", "images", dbCategory.Image);

                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }

                    filePath = Path.Combine(_env.WebRootPath, "assets", "images", fileName);

                    using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await category.File.CopyToAsync(fileStream);
                    }

                    category.Image = fileName;
                    dbCategory.Image = category.Image;
                    dbCategory.ParentId = null;
                }

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
                dbCategory.Image = null;
                dbCategory.ParentId = category.ParentId;
            }

            dbCategory.Name = category.Name.Trim();
            dbCategory.UpdatedBy = "User";
            dbCategory.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        //7.Delete(Get)
        public async Task<IActionResult> Delete(int?id) 
        {
            if (id == null) return BadRequest();

            Category category = await _context.Categories
                .Include(c => c.Children.Where(ch => ch.IsDeleted == false)).ThenInclude(c => c.Products.Where(p => p.IsDeleted == false))
                .Include(c => c.Products.Where(p => p.IsDeleted == false))
                .FirstOrDefaultAsync(c => c.IsDeleted == false && c.Id == id);
            if (category == null) return NotFound();

            return View(category);
        }
        //8.DeleteCategory
        public async Task<IActionResult> DeleteCategory(int? id)
        {
            if (id == null) return BadRequest();
            Category category = await _context.Categories
                .Include(c => c.Children.Where(ch => ch.IsDeleted == false)).ThenInclude(c => c.Products.Where(p => p.IsDeleted == false))
                .Include(c => c.Products.Where(p => p.IsDeleted == false))
                .FirstOrDefaultAsync(c => c.IsDeleted == false && c.Id == id);

            if (category == null) return NotFound();

            if (category.Products != null && category.Products.Count() > 0)
            {
                return BadRequest();
            }

            if (category.Children != null && category.Children.Count() > 0)
            {
                foreach (Category child in category.Children)
                {
                    if (child.Products != null && child.Products.Count() > 0)
                    {
                        return BadRequest();
                    }
                }

                return BadRequest();
            }

            category.IsDeleted = true;
            category.DeletedAt = DateTime.Now;
            category.DeletedBy = "User";
            await _context.SaveChangesAsync();

            if (!string.IsNullOrWhiteSpace(category.Image))
            {
                string filePath = Path.Combine(_env.WebRootPath, "assets", "images", category.Image);

                if (System.IO.File.Exists(filePath))
                {
                   System.IO.File.Delete(filePath); 
                }
                
            }

            return RedirectToAction(nameof(Index));

        }

    }
}
