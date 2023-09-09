using Allup.DataAccessLayer;
using Allup.Models;
using Allup.ViewModels.BasketVMs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Allup.Controllers
{
    public class CartController : Controller
    {
        private readonly AppDbContext _context;

        public CartController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            List<BasketVM> basketVMs = new List<BasketVM>();
            string? basket = Request.Cookies["basket"];
            if (string.IsNullOrWhiteSpace(basket))
            {
                return View(basketVMs);
            }
            else
            {
                basketVMs = JsonConvert.DeserializeObject<List<BasketVM>>(basket);
                foreach (BasketVM basketVM in basketVMs)
                {
                    Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == basketVM.Id);
                    basketVM.Title = product.Title;
                    basketVM.Image = product.MainImage;
                    basketVM.Price = product.DiscountedPrice > 0 ? product.DiscountedPrice : product.Price;
                    basketVM.ExTax = product.ExTag;

                }
            }

            return View(basketVMs);
        }
        public async Task<IActionResult> RemoveCart(int? id) 
        {
            if (id == null) return BadRequest("Id is required");

            string? basket = Request.Cookies["basket"];

            List<BasketVM>? ProductsInCart = JsonConvert.DeserializeObject<List<BasketVM>>(basket);

            bool IsRemoved = ProductsInCart.Remove(ProductsInCart.FirstOrDefault(p => p.Id == id));

            if (!IsRemoved) return NotFound("This Id does not exist in this basket");

            basket = JsonConvert.SerializeObject(ProductsInCart);

            Response.Cookies.Append("basket", basket);

            foreach (BasketVM basketVM in ProductsInCart)
            {
                Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == basketVM.Id);
                basketVM.Title = product.Title;
                basketVM.Image = product.MainImage;
                basketVM.Price = product.DiscountedPrice > 0 ? product.DiscountedPrice : product.Price;
                basketVM.ExTax = product.ExTag;

            }
            return PartialView("_CartPartial",ProductsInCart);
        }
    }
}
