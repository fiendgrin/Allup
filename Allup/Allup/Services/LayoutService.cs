using Allup.DataAccessLayer;
using Allup.Interfaces;
using Allup.Models;
using Allup.ViewModels.BasketVMs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Allup.Services
{
    public class LayoutService : ILayoutService
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly UserManager<AppUser> _userManager;

        public LayoutService(AppDbContext context, IHttpContextAccessor contextAccessor, UserManager<AppUser> userManager)
        {
            _context = context;
            _contextAccessor = contextAccessor;
            _userManager = userManager;
        }

        public async Task<List<BasketVM>> GetBasketsAsync()
        {


            List<BasketVM> basketVMs = null;


            if (_contextAccessor.HttpContext.User.Identity.IsAuthenticated && _contextAccessor.HttpContext.User.IsInRole("Member"))
            {
                AppUser appUser = await _userManager.Users
                    .Include(u => u.Baskets.Where(b => b.IsDeleted == false)).ThenInclude(b=>b.Product)
                    .FirstOrDefaultAsync(u => u.UserName == _contextAccessor.HttpContext.User.Identity.Name);

                basketVMs = new List<BasketVM>();

                if (appUser.Baskets != null && appUser.Baskets.Count() > 0)
                {

                    foreach (Basket basket in appUser.Baskets)
                    {
                        
                        BasketVM basketVM = new BasketVM 
                        {
                            Id = (int)basket.ProductId,
                            Count = basket.Count,
                            ExTax = basket.Product.ExTag,
                            Image = basket.Product.MainImage,
                            Price = basket.Product.DiscountedPrice > 0 ? basket.Product.DiscountedPrice: basket.Product.Price,
                            Title = basket.Product.Title
                        };

                        basketVMs.Add(basketVM);
                    }

                }

                string cookie = JsonConvert.SerializeObject(basketVMs);
                _contextAccessor.HttpContext.Response.Cookies.Append("basket", cookie);

            }
            else
            {
                string cookie = _contextAccessor.HttpContext.Request.Cookies["basket"];

                if (!string.IsNullOrWhiteSpace(cookie))
                {
                    basketVMs = JsonConvert.DeserializeObject<List<BasketVM>>(cookie);
                }
                else
                {
                    basketVMs = new List<BasketVM>();
                }

                foreach (BasketVM basketVM in basketVMs)
                {
                    Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == basketVM.Id);
                    basketVM.Title = product.Title;
                    basketVM.Image = product.MainImage;
                    basketVM.Price = product.DiscountedPrice > 0 ? product.DiscountedPrice : product.Price;
                    basketVM.ExTax = product.ExTag;

                }
            }

            

            return basketVMs;
        }

        public async Task<List<Category>> GetCategoriesAsync()
        {
            List<Category> categories = await _context.Categories.
                Include(c => c.Children.Where(ch => ch.IsDeleted == false)).
                Where(c => c.IsDeleted == false && c.IsMain == true).ToListAsync();
            return categories;
        }

        public async Task<Dictionary<string, string>> GetSettingsAsync()
        {
            Dictionary<string, string> settings = await _context.Settings.ToDictionaryAsync(s => s.Key, s => s.Value);

            return settings;
        }
    }
}
