using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using GE_Flipper.Models;

namespace GE_Flipper.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<ItemCategory> ItemCategories { get; set; }
        public DbSet<Items> Items { get; set; }
    }
}
