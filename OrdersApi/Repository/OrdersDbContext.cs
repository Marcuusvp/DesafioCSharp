using Microsoft.EntityFrameworkCore;
using OrdersApi.Models;

namespace OrdersApi.Repository
{
    public class OrdersDbContext : DbContext
    {
        public OrdersDbContext(DbContextOptions<OrdersDbContext> options)
            : base(options)
        {
        }
        public DbSet<Item> Items { get; set; }
        public DbSet<Order> Orders { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Ignore<OrderItem>();
            // Configure Item entity
            modelBuilder.Entity<Item>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired();
                entity.Property(e => e.Price)
                      .HasColumnType("decimal(18,2)")
                      .IsRequired();
            });

            // Configure Order entity
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.User).IsRequired();
                entity.Property(e => e.Total)
                      .HasColumnType("decimal(18,2)")
                      .IsRequired();
                entity.Property(e => e.Status).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();

                entity.OwnsMany(e => e.Items, a =>
                {
                    a.WithOwner().HasForeignKey("OrderId");
                    a.Property<int>("Id");
                    a.HasKey("Id");

                    a.Property(e => e.Quantity).IsRequired();

                    // Configure the Item relationship
                    a.HasOne(e => e.Item)
                     .WithMany()
                     .HasForeignKey("ItemId")
                     .IsRequired();
                });
            });
        }
    }
}
