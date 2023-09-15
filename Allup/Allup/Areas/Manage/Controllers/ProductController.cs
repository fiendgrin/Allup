using Allup.DataAccessLayer;
using Allup.Models;
using Allup.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Allup.Areas.Manage.Controllers
{
    [Area("manage")]
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ProductController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public IActionResult Index(int currentPage = 1)
        {
            IQueryable<Product> queries = _context.Products
                .Include(p => p.Category)
                .Include(p=>p.Brand)
                .Include(p=>p.ProductTags.Where(pt=>pt.IsDeleted == false)).ThenInclude(pt=>pt.Tag)
                .Where(p=>p.IsDeleted == false)
                .OrderByDescending(p=>p.Id);

            return View(PageNatedList<Product>.Create(queries, currentPage,5,10));
        }

        public async Task<IActionResult> Create() 
        {
            ViewBag.Categories = await _context.Categories.Where(c => c.IsDeleted == false).ToListAsync();
            ViewBag.Brands = await _context.Brands.Where(b => b.IsDeleted == false).ToListAsync();
            ViewBag.Tags = await _context.Tags.Where(t => t.IsDeleted == false).ToListAsync();

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Product product) 
        {
            ViewBag.Categories = await _context.Categories.Where(c => c.IsDeleted == false).ToListAsync();
            ViewBag.Brands = await _context.Brands.Where(b => b.IsDeleted == false).ToListAsync();
            ViewBag.Tags = await _context.Tags.Where(t => t.IsDeleted == false).ToListAsync();

            if (!ModelState.IsValid) return View(product);

            if (product.CategoryId == null || !await _context.Categories.AnyAsync(c=>c.IsDeleted == false && c.Id == product.CategoryId))
            {
                ModelState.AddModelError("CategoryId",$"{product.CategoryId} is Incorrect");
                return View(product);
            }

            if (product.BrandId != null && !await _context.Brands.AnyAsync(b=> b.IsDeleted == false && b.Id == product.Id))
            {
                ModelState.AddModelError("BrandId", $"{product.BrandId} is Incorrect");
                return View(product);
            }

            List<ProductTag> productTags = new List<ProductTag>();
            if (product.TagIds != null && product.TagIds.Count() > 0)
            {
                foreach (int tagId in product.TagIds) 
                {
                    if (!await _context.Tags.AnyAsync(t=>t.IsDeleted == false && t.Id == tagId))
                    {
                        ModelState.AddModelError("TagIds", $"Tag Id{tagId} is Incorrect");
                        return View(product);
                    }

                    ProductTag productTag = new ProductTag 
                    {
                        TagId = tagId
                    };
                    productTags.Add(productTag);
                }
            }

            product.ProductTags = productTags;



            if (product.Files == null)
            {
                ModelState.AddModelError("Files", $"Minimum 1 file");
                return View(product);
            }
            if (product.Files.Count() > 10)
            {
                ModelState.AddModelError("Files", $"Maximum of 10 files is allowed");
                return View(product);
            }

            foreach(IFormFile file in product.Files) 
            {
                if (!file.ContentType.Contains("image/"))
                {
                    ModelState.AddModelError("Files", "File type should be .jpg or .jpeg");
                    return View(file);
                }

                if ((file.Length / 1024) > 300)
                {
                    ModelState.AddModelError("Files", "File can't be larger than 300kb");
                    return View(file);
                }
            }

            List<ProductImage> productImages = new List<ProductImage>();

            foreach (IFormFile file in product.Files)
            {
                string fileName = DateTime.UtcNow.AddHours(4).ToString("yyyyMMddHHmmssfff") + file.FileName.
                    Substring(file.FileName.LastIndexOf("."));

                string filePath = Path.Combine(_env.WebRootPath, "assets", "images","product", fileName);

                using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                ProductImage productImage = new ProductImage
                {
                    Image = fileName
                };
                productImages.Add(productImage);
            }

            product.ProductImages = productImages;

            if (product.MainFile != null)
            {
                if (!product.MainFile.ContentType.Contains("image/"))
                {
                    ModelState.AddModelError("MainFile", "Main File type should be .jpg or .jpeg");
                    return View(product.MainFile);
                }

                if ((product.MainFile.Length / 1024) > 300)
                {
                    ModelState.AddModelError("MainFile", "Main File can't be larger than 300kb");
                    return View(product.MainFile);
                }

                string fileName = DateTime.UtcNow.AddHours(4).ToString("yyyyMMddHHmmssfff") + product.MainFile.FileName.
                   Substring(product.MainFile.FileName.LastIndexOf("."));

                string filePath = Path.Combine(_env.WebRootPath, "assets", "images", "product", fileName);

                using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await product.MainFile.CopyToAsync(fileStream);
                }

                product.MainImage = fileName;
            }
            else 
            {
                ModelState.AddModelError("MainFile", $"Main File required");
                return View(product);
            }

            if (product.HoverFile != null)
            {
                if (!product.HoverFile.ContentType.Contains("image/"))
                {
                    ModelState.AddModelError("HoverFile", "Hover File type should be .jpg or .jpeg");
                    return View(product.HoverFile);
                }

                if ((product.HoverFile.Length / 1024) > 300)
                {
                    ModelState.AddModelError("HoverFile", "Hover File can't be larger than 300kb");
                    return View(product.HoverFile);
                }

                string fileName = DateTime.UtcNow.AddHours(4).ToString("yyyyMMddHHmmssfff") + product.HoverFile.FileName.
                   Substring(product.HoverFile.FileName.LastIndexOf("."));

                string filePath = Path.Combine(_env.WebRootPath, "assets", "images", "product", fileName);

                using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await product.HoverFile.CopyToAsync(fileStream);
                }

                product.HoverImage = fileName;
            }
            else
            {
                ModelState.AddModelError("HoverFile", $"Hover File required");
                return View(product);
            }

            Category category = await _context.Categories.FirstOrDefaultAsync(c=>c.Id == product.CategoryId);
            Brand brand = await _context.Brands.FirstOrDefaultAsync(b => b.Id == product.BrandId);


            string seria = (category.Name.Substring(0, 2) + brand.Name.Substring(0, 2)).ToLower();

            Product prod = await _context.Products.Where(p => p.Seria.ToLower() == seria).OrderByDescending(p => p.Number).FirstOrDefaultAsync(); ;

            int? number = prod != null ? prod.Number + 1 : 1;

            product.Seria = seria;
            product.Number = number;



            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();


            return RedirectToAction(nameof(Index));
        }
    }
}
