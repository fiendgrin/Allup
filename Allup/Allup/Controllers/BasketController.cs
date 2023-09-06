using Allup.DataAccessLayer;
using Allup.Models;
using Allup.ViewModels.BasketVMs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Allup.Controllers
{
    public class BasketController : Controller
    {
        private readonly AppDbContext _context;

        public BasketController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> AddBasket(int? id)
        {
            if (id == null) return BadRequest("Id is required");

            Product product = await _context.Products
                .Include(p => p.ProductImages.Where(pi => pi.IsDeleted == false)).
                FirstOrDefaultAsync(p => p.IsDeleted == false && p.Id == id);

            if (product == null) return NotFound("This Id does not exist");

            string basket = Request.Cookies["Basket"];

            if (string.IsNullOrWhiteSpace(basket))
            {
                List<BasketVM> basketVMs = JsonConvert.DeserializeObject<List<BasketVM>>(basket);

                if (basketVMs.Exists(b => b.Id == id))
                {
                    basketVMs.Find(b => b.Id == id).Count += 1;
                }
                else
                {
                    BasketVM basketVM = new BasketVM
                    {
                        Id = product.Id,
                        Title = product.Title,
                        ExTax = product.ExTag,
                        Image = product.MainImage,
                        Price = product.Price,
                        Count = 1
                    };
                    basketVMs.Add(basketVM);
                }

                string cookie = JsonConvert.SerializeObject(basketVMs);

                Response.Cookies.Append("Basket", cookie);
            }
            else
            {
                BasketVM basketVM = new BasketVM
                {
                    Id = product.Id,
                    Title = product.Title,
                    ExTax = product.ExTag,
                    Image = product.MainImage,
                    Price = product.Price,
                    Count = 1
                };
                List<BasketVM> basketVMs = new List<BasketVM> { basketVM };

                string cookie = JsonConvert.SerializeObject(basketVMs);

                Response.Cookies.Append("Basket", cookie);
            }

            return Ok();
        }

        public IActionResult GetCookies()
        {
            string cookie = Request.Cookies["Basket"];

            return Ok();
        }
    }
}
