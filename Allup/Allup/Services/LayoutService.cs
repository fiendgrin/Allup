using Allup.DataAccessLayer;
using Allup.Models;
using Microsoft.EntityFrameworkCore;

namespace Allup.Services
{
    public class LayoutService
    {
        private readonly AppDbContext _context;

        public LayoutService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Setting>> GetSettingsAsync() 
        { 
            List<Setting> settings = await _context.Settings.ToListAsync();

            return settings;
        }
    }
}
