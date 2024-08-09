using Microsoft.EntityFrameworkCore;

namespace OnlineQRMenuApp.Models
{
    public class OnlineCoffeeManagementContext : DbContext
    {
        public OnlineCoffeeManagementContext(DbContextOptions<OnlineCoffeeManagementContext> options)
            : base(options)
        {
        }

        public DbSet<CustomizationGroup> CustomizationGroups { get; set; }
        public DbSet<MenuItem> MenuItems { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<CoffeeShop> CoffeeShops { get; set; }
        public DbSet<LoyaltyProgram> LoyaltyPrograms { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<MenuItemCustomization> MenuItemCustomizations { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<OrderItemCustomization> OrderItemCustomizations { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.MenuItem)
                .WithMany(mi => mi.OrderItems)
                .HasForeignKey(oi => oi.MenuItemId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<OrderItemCustomization>()
                .HasOne(oic => oic.OrderItem)
                .WithMany(oi => oi.Customizations)
                .HasForeignKey(oic => oic.OrderItemId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OrderItemCustomization>()
                .HasOne(oic => oic.MenuItemCustomization)
                .WithMany()
                .HasForeignKey(oic => oic.MenuItemCustomizationId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MenuItem>()
                .HasOne(mi => mi.CoffeeShop)
                .WithMany(cs => cs.MenuItems)
                .HasForeignKey(mi => mi.CoffeeShopId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MenuItem>()
                .HasOne(mi => mi.Category)
                .WithMany(c => c.MenuItems)
                .HasForeignKey(mi => mi.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<LoyaltyProgram>()
                .HasOne(lp => lp.User)
                .WithMany(u => u.LoyaltyPrograms)
                .HasForeignKey(lp => lp.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.CoffeeShop)
                .WithMany(cs => cs.Orders)
                .HasForeignKey(o => o.CoffeeShopId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
