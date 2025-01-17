using MauiClientApp.Models;
using Microsoft.EntityFrameworkCore;

namespace MauiClientApp
{
    public class AppDbContext : DbContext
    {
        public DbSet<TokenModel> TokenModels { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
    }
}
