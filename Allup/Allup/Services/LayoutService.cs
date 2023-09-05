using Allup.DataAccessLayer;
using Allup.Interfaces;
using Allup.Models;
using Microsoft.EntityFrameworkCore;

namespace Allup.Services
{
    public class LayoutService : ILayoutService
    {
        private readonly AppDbContext _context;

        public LayoutService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Category>> GetCategoriesAsync()
        {
            List<Category> categories = await _context.Categories.
                Include(c=>c.Children.Where(ch=>ch.IsDeleted==false)).
                Where(c=>c.IsDeleted == false && c.IsMain).ToListAsync();
            return categories;
        }

        public async Task<Dictionary<string, string>> GetSettingsAsync() 
        { 
            Dictionary<string,string> settings = await _context.Settings.ToDictionaryAsync(s=>s.Key,s=>s.Value);

            return settings;
        }
    }
}
