using Allup.DataAccessLayer;
using Allup.Interfaces;
using Allup.Models;
using Allup.ViewModels.BasketVMs;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Allup.Services
{
    public class LayoutService : ILayoutService
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _contextAccessor;

        public LayoutService(AppDbContext context, IHttpContextAccessor contextAccessor)
        {
            _context = context;
            _contextAccessor = contextAccessor;
        }

        public async Task<List<BasketVM>> GetBasketsAsync()
        {
            string cookie = _contextAccessor.HttpContext.Request.Cookies["basket"];

            List<BasketVM> basketVMs = null;

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
