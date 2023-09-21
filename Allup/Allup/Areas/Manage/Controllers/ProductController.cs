using Allup.DataAccessLayer;
using Allup.Helpers;
using Allup.Models;
using Allup.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Drawing.Drawing2D;

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
        //1.Index
        //2.Detail
        //3.Create(Get)
        //4.Create(Post)
        //5.Update(Get)
        //6.Update(Post)
        //7.DeleteImage
        //8.Delete
        //9.DeleteProduct

        //=============================================================

        //1.Index
        public IActionResult Index(int currentPage = 1)
        {
            IQueryable<Product> queries = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.ProductTags.Where(pt => pt.IsDeleted == false)).ThenInclude(pt => pt.Tag)
                .Where(p => p.IsDeleted == false)
                .OrderByDescending(p => p.Id);

            return View(PageNatedList<Product>.Create(queries, currentPage, 5, 10));
        }

        //2.Detail
        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null) return BadRequest();
            Product product = await _context.Products
                .Include(p => p.Brand).Include(p => p.Category)
                .Include(p => p.ProductImages.Where(pi => pi.IsDeleted == false))
                .Include(p => p.ProductTags.Where(pi => pi.IsDeleted == false)).ThenInclude(pt => pt.Tag)
                .FirstOrDefaultAsync(p => p.IsDeleted == false && p.Id == id);

            if (product == null) return NotFound();

            return View(product);
        }

        //3.Create(Get)
        [Authorize(Roles ="Admin,SuperAdmin")]
        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await _context.Categories.Where(c => c.IsDeleted == false).ToListAsync();
            ViewBag.Brands = await _context.Brands.Where(b => b.IsDeleted == false).ToListAsync();
            ViewBag.Tags = await _context.Tags.Where(t => t.IsDeleted == false).ToListAsync();

            return View();
        }

        //4.Create(Post)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> Create(Product product)
        {
            ViewBag.Categories = await _context.Categories.Where(c => c.IsDeleted == false).ToListAsync();
            ViewBag.Brands = await _context.Brands.Where(b => b.IsDeleted == false).ToListAsync();
            ViewBag.Tags = await _context.Tags.Where(t => t.IsDeleted == false).ToListAsync();

            if (!ModelState.IsValid) return View(product);

            if (product.CategoryId == null || !await _context.Categories.AnyAsync(c => c.IsDeleted == false && c.Id == product.CategoryId))
            {
                ModelState.AddModelError("CategoryId", $"{product.CategoryId} is Incorrect");
                return View(product);
            }

            if (product.BrandId != null && !await _context.Brands.AnyAsync(b => b.IsDeleted == false && b.Id == product.BrandId))
            {
                ModelState.AddModelError("BrandId", $"{product.BrandId} is Incorrect");
                return View(product);
            }


            List<ProductTag> productTags = new List<ProductTag>();
            if (product.TagIds != null && product.TagIds.Count() > 0)
            {
                foreach (int tagId in product.TagIds)
                {
                    if (!await _context.Tags.AnyAsync(t => t.IsDeleted == false && t.Id == tagId))
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

            List<ProductImage> productImages = new List<ProductImage>();

            foreach (IFormFile file in product.Files)
            {

                ProductImage productImage = new ProductImage
                {
                    Image = await file.Save(_env.WebRootPath, new string[] { "assets", "images", "product" })
                };
                productImages.Add(productImage);
            }

            product.ProductImages = productImages;

            if (product.MainFile != null)
            {
                product.MainImage = await product.MainFile.Save(_env.WebRootPath, new string[] { "assets", "images", "product" });
            }
            else
            {
                ModelState.AddModelError("MainFile", $"Main File required");
                return View(product);
            }

            if (product.HoverFile != null)
            {
                product.HoverImage = await product.HoverFile.Save(_env.WebRootPath, new string[] { "assets", "images", "product" });
            }
            else
            {
                ModelState.AddModelError("HoverFile", $"Hover File required");
                return View(product);
            }

            Category category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == product.CategoryId);
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

        //5.Update(Get)
        public async Task<IActionResult> Update(int? id)
        {
            ViewBag.Categories = await _context.Categories.Where(c => c.IsDeleted == false).ToListAsync();
            ViewBag.Brands = await _context.Brands.Where(b => b.IsDeleted == false).ToListAsync();
            ViewBag.Tags = await _context.Tags.Where(t => t.IsDeleted == false).ToListAsync();

            if (id == null) return BadRequest();

            Product? product = await _context.Products
                .Include(p => p.ProductImages.Where(pi => pi.IsDeleted == false))
                //.Include(p => p.ProductTags.Where(pi => pi.IsDeleted == false))
                .FirstOrDefaultAsync(p => p.Id == id && p.IsDeleted == false);

            if (product == null) return BadRequest();

            product.TagIds = await _context.ProductTags
                .Where(pt => pt.ProductId == product.Id && pt.IsDeleted == false)
                .Select(x => x.TagId).ToListAsync();

            return View(product);
        }

        //6.Update(Post)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id, Product product)
        {
            ViewBag.Categories = await _context.Categories.Where(c => c.IsDeleted == false).ToListAsync();
            ViewBag.Brands = await _context.Brands.Where(b => b.IsDeleted == false).ToListAsync();
            ViewBag.Tags = await _context.Tags.Where(t => t.IsDeleted == false).ToListAsync();


            if (id == null) return BadRequest();

            if (product.Id != id) return BadRequest();


            Product? dbProduct = await _context.Products
                .Include(p => p.ProductImages.Where(pi => pi.IsDeleted == false))
                .Include(p => p.ProductTags.Where(pi => pi.IsDeleted == false))
                .FirstOrDefaultAsync(p => p.Id == product.Id && p.IsDeleted == false);

            if (dbProduct == null) return NotFound();

            dbProduct.TagIds = product.TagIds;

            if (!ModelState.IsValid) return View(product);

            int canUpload = 10 - dbProduct.ProductImages.Count;

            if (product.Files != null && product.Files.Count() > canUpload)
            {
                ModelState.AddModelError("Files", $"Maximum of 10 Images, you only have space for {canUpload} more");
                return View(dbProduct);
            }

            if (product.CategoryId == null || !await _context.Categories.AnyAsync(c => c.IsDeleted == false && c.Id == product.CategoryId))
            {
                ModelState.AddModelError("CategoryId", $"{product.CategoryId} is Incorrect");
                return View(dbProduct);
            }

            if (product.BrandId != null && !await _context.Brands.AnyAsync(b => b.IsDeleted == false && b.Id == product.BrandId))
            {
                ModelState.AddModelError("BrandId", $"{product.BrandId} is Incorrect");
                return View(dbProduct);
            }

            if (dbProduct.ProductTags != null && dbProduct.ProductTags.Count() > 0)
            {
                foreach (ProductTag productTag in dbProduct.ProductTags)
                {
                    productTag.IsDeleted = true;
                    productTag.DeletedBy = "User";
                    productTag.DeletedAt = DateTime.Now;

                }
            }

            List<ProductTag> productTags = new List<ProductTag>();
            if (product.TagIds != null && product.TagIds.Count() > 0)
            {
                foreach (int tagId in product.TagIds)
                {
                    if (!await _context.Tags.AnyAsync(t => t.IsDeleted == false && t.Id == tagId))
                    {
                        ModelState.AddModelError("TagIds", $"Tag Id{tagId} is Incorrect");
                        return View(dbProduct);
                    }

                    ProductTag productTag = new ProductTag
                    {
                        TagId = tagId
                    };
                    productTags.Add(productTag);
                }
            }

            dbProduct.ProductTags.AddRange(productTags);

            if (product.MainFile != null)
            {
                string filePath = Path.Combine(_env.WebRootPath, "assets", "images", "product", dbProduct.MainImage);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
                product.MainImage = await product.MainFile.Save(_env.WebRootPath, new string[] { "assets", "images", "product" });
                dbProduct.MainImage = product.MainImage;
            }

            if (product.HoverFile != null)
            {
                string filePath = Path.Combine(_env.WebRootPath, "assets", "images", "product", dbProduct.HoverImage);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
                product.HoverImage = await product.HoverFile.Save(_env.WebRootPath, new string[] { "assets", "images", "product" });
                dbProduct.HoverImage = product.HoverImage;
            }

            List<ProductImage> productImages = new List<ProductImage>();

            if (product.Files != null && product.Files.Count() > 0)
            {
                foreach (IFormFile file in product.Files)
                {
                    ProductImage productImage = new ProductImage
                    {
                        Image = await file.Save(_env.WebRootPath, new string[] { "assets", "images", "product" })
                    };
                    productImages.Add(productImage);
                }

                dbProduct.ProductImages.AddRange(productImages);


            }

            Category category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == product.CategoryId);
            Brand brand = await _context.Brands.FirstOrDefaultAsync(b => b.Id == product.BrandId);


            string seria = (category.Name.Substring(0, 2) + brand.Name.Substring(0, 2)).ToLower();

            Product prod = await _context.Products.Where(p => p.Seria.ToLower() == seria).OrderByDescending(p => p.Number).FirstOrDefaultAsync(); ;

            int? number = prod != null ? prod.Number + 1 : 1;

            product.Seria = seria;
            product.Number = number;

            dbProduct.Seria = product.Seria;
            dbProduct.Number = product.Number;

            dbProduct.Title = product.Title.Trim();

            dbProduct.Description = product.Description.Trim();

            dbProduct.SmallDescription = product.SmallDescription.Trim();

            dbProduct.ExTag = product.ExTag;

            dbProduct.Price = product.Price;

            dbProduct.DiscountedPrice = product.DiscountedPrice;

            dbProduct.IsBestSeller = product.IsBestSeller;
            dbProduct.IsFeatured = product.IsFeatured;
            dbProduct.IsNewArrival = product.IsNewArrival;

            dbProduct.BrandId = product.BrandId;

            dbProduct.CategoryId = product.CategoryId;

            dbProduct.UpdatedBy = "User";

            dbProduct.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();


            return RedirectToAction(nameof(Index));
        }
        //7.DeleteImage
        public async Task<IActionResult> DeleteImage(int? id, int? imageId)
        {
            if (id == null) return BadRequest();

            if (imageId == null) return BadRequest();

            Product? product = await _context.Products
                .Include(p => p.ProductImages.Where(pi => pi.IsDeleted == false))
                .FirstOrDefaultAsync(p => p.Id == id && p.IsDeleted == false);

            if (product == null) return NotFound();
            
                
            

            if (!product.ProductImages.Any(pi => pi.Id == imageId)) return NotFound();

            if (product.ProductImages.Count <= 1) return BadRequest();

            product.ProductImages.FirstOrDefault(pi => pi.Id == imageId).IsDeleted = true;
            product.ProductImages.FirstOrDefault(pi => pi.Id == imageId).DeletedAt = DateTime.Now;
            product.ProductImages.FirstOrDefault(pi => pi.Id == imageId).DeletedBy = "User";

            string fileName = product.ProductImages.FirstOrDefault(pi => pi.Id == imageId).Image;
            await _context.SaveChangesAsync();
            string filePath = Path.Combine(_env.WebRootPath, "assets", "images", "product", fileName);
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            return PartialView("_DeleteImagePartial", product.ProductImages.Where(p => p.IsDeleted == false).ToList());
        }
        
        //8.Delete
        public async Task<IActionResult> Delete(int? id) 
        {
            if (id == null) return BadRequest();
            Product product = await _context.Products
                .Include(p => p.Brand).Include(p => p.Category)
                .Include(p => p.ProductImages.Where(pi => pi.IsDeleted == false))
                .Include(p => p.ProductTags.Where(pi => pi.IsDeleted == false)).ThenInclude(pt => pt.Tag)
                .FirstOrDefaultAsync(p => p.IsDeleted == false && p.Id == id);

            if (product == null) return NotFound();

            return View(product);
        }
        //9.DeleteProduct
        public async Task<IActionResult> DeleteProduct(int? id)
        {
            if (id == null) return BadRequest();

            Product product = await _context.Products
               .Include(p => p.Brand).Include(p => p.Category)
               .Include(p => p.ProductImages.Where(pi => pi.IsDeleted == false))
               .Include(p => p.ProductTags.Where(pi => pi.IsDeleted == false)).ThenInclude(pt => pt.Tag)
               .FirstOrDefaultAsync(p => p.IsDeleted == false && p.Id == id);

            if (product == null) return NotFound();


            if (product.ProductTags != null && product.ProductTags.Count > 0)
            {
                foreach (ProductTag productTag in product.ProductTags)
                {
                    productTag.IsDeleted = true;
                    productTag.DeletedBy = "User";
                    productTag.DeletedAt = DateTime.Now;
                }
            }

            if (product.ProductImages != null && product.ProductImages.Count > 0)
            {
                foreach (ProductImage productImage in product.ProductImages)
                {
                    productImage.IsDeleted = true;
                    productImage.DeletedBy = "User";
                    productImage.DeletedAt = DateTime.Now;
                }
            }

            product.IsDeleted = true;
            product.DeletedBy = "User";
            product.DeletedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            if (!string.IsNullOrWhiteSpace(product.MainImage))
            {
                string filePath = Path.Combine(_env.WebRootPath, "assets", "images", "product", product.MainImage);

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

            }

            if (!string.IsNullOrWhiteSpace(product.HoverImage))
            {
                string filePath = Path.Combine(_env.WebRootPath, "assets", "images", "product", product.HoverImage);

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

            }

            foreach (ProductImage productImage1 in product.ProductImages)
            {
                if (!string.IsNullOrWhiteSpace(productImage1.Image))
                {
                    string filePath = Path.Combine(_env.WebRootPath, "assets", "images", "product", productImage1.Image);

                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }

                }
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
