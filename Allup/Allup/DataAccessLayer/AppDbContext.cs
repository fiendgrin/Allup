using Allup.Models;
using Microsoft.EntityFrameworkCore;

namespace Allup.DataAccessLayer
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            
        }

        public DbSet<Slider> Sliders { get; set; }
    }
}
