
using Microsoft.EntityFrameworkCore;
using TranNgocThu_2122110387.Model;
namespace TranNgocThu_2122110387.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Product> Products { get; set; }
    }
}
