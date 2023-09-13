using Allup.DataAccessLayer;
using Allup.Models;
using Allup.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Allup.Areas.Manage.Controllers
{
    [Area("manage")]
    public class BrandController : Controller
    {
        private readonly AppDbContext _context;

        public BrandController(AppDbContext context)
        {
            _context = context;
        }

        //1.Index
        //2.Detail
        //3.Create(Get)
        //4.Create(Post)
        //5.Update(Get)
        //6.Update(Post)
        //7.Delete(Get)
        //8.DeleteBrand(Get)

        //=============================================================

        //1.Index
        public async Task<IActionResult> Index(int currentPage = 1)
        {
            IQueryable<Brand> queries = _context.Brands
                .Include(b=>b.Products.Where(p=>p.IsDeleted == false))
                .Where(b => b.IsDeleted == false)
                .OrderByDescending(b=>b.Id);
            return View(PageNatedList<Brand>.Create(queries,currentPage,5,8));
        }

        //2.Detail
        public async Task<IActionResult> Detail(int? id) 
        {
            if (id == null) return BadRequest();

            Brand brand = await _context.Brands
                .Include(b=>b.Products.Where(p=>p.IsDeleted == false))
                .FirstOrDefaultAsync(b=>b.IsDeleted==false && b.Id == id);
            if (brand == null) return NotFound(); 

            return View(brand);
        }

        //3.Create(Get)
        public IActionResult Create() 
        {
            return View();
        }

        //4.Create(Post)
        [HttpPost]
        public async Task<IActionResult> Create(Brand brand) 
        {
            if (!ModelState.IsValid) return View(brand);

            if (await _context.Brands.AnyAsync(b => b.IsDeleted == false && b.Name.ToLower() == brand.Name.Trim().ToLower()))
            {
                ModelState.AddModelError("Name",$"{brand.Name} already exists");
                return View(brand);
            }

            brand.Name = brand.Name.Trim();

            await _context.Brands.AddAsync(brand);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        //5.Update(Get)
        public async Task<IActionResult> Update(int? id) 
        {
            if (id == null) return BadRequest();

            Brand brand = await _context.Brands
                .FirstOrDefaultAsync(b => b.IsDeleted == false && b.Id == id);
            if (brand == null) return NotFound();

            return View(brand);
        }

        //6.Update(Post)
        [HttpPost]
        public async Task<IActionResult> Update(int? id ,Brand brand) 
        {
            if (id == null) return BadRequest();

            if (id != brand.Id) return BadRequest();

            if (!ModelState.IsValid) return View(brand);


            Brand DbBrand = await _context.Brands
                .FirstOrDefaultAsync(b => b.IsDeleted == false && b.Id == id);

            if (DbBrand == null) return NotFound();


            if (await _context.Brands.AnyAsync(b => b.IsDeleted == false && b.Name.ToLower() == brand.Name.Trim().ToLower() && b.Id != DbBrand.Id))
            {
                ModelState.AddModelError("Name", $"{brand.Name} already exists");
                return View(brand);
            }

            DbBrand.Name = brand.Name.Trim();
            DbBrand.UpdatedBy = "User";
            DbBrand.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        //7.Delete(Get)
        public async Task<IActionResult> Delete(int? id) 
        {
            if (id == null) return BadRequest();

            Brand brand = await _context.Brands
                .Include(b => b.Products.Where(p => p.IsDeleted == false))
                .FirstOrDefaultAsync(b => b.IsDeleted == false && b.Id == id);
            if (brand == null) return NotFound();

            return View(brand);
        }

        //8.DeleteBrand(Get)
        public async Task<IActionResult> DeleteBrand(int? id)
        {
            if (id == null) return BadRequest();

            Brand brand = await _context.Brands
                .Include(b => b.Products.Where(p => p.IsDeleted == false))
                .FirstOrDefaultAsync(b => b.IsDeleted == false && b.Id == id);
            if (brand == null) return NotFound();

            brand.IsDeleted = true;
            brand.DeletedBy = "User";
            brand.DeletedAt = DateTime.Now;

            if (brand.Products != null && brand.Products.Count()>0)
            {
                foreach (Product product in brand.Products)
                {
                    product.BrandId = null;
                }
            }
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
