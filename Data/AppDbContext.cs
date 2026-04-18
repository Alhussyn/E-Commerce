using E_Commerce.Models;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Data
{
    public class AppDbContext:DbContext
    {
        public AppDbContext()
        {
            
        }
        public AppDbContext(DbContextOptions<AppDbContext> options):base (options)
        {
            
        }
        public DbSet<Product> Products {  get; set; }
        public DbSet<Order> Orders {  get; set; }
        public DbSet<User> Users {  get; set; }
        public DbSet<OrderItem> OrderItems {  get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=.;Database=ECommerceDB.ConsoleApp.NewDb;Trusted_Connection=True;TrustServerCertificate=true");
            base.OnConfiguring(optionsBuilder);
        }
        
    }
}
