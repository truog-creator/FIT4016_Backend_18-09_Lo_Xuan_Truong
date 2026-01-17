using Microsoft.EntityFrameworkCore;
using OrderManagement.Models;
using System;

namespace OrderManagement.Data
{
    public class OrderManagementContext : DbContext
    {
        public OrderManagementContext(DbContextOptions<OrderManagementContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ==================== Product Configuration ====================
            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("Products");

                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Sku)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Description)
                    .HasMaxLength(1000);

                entity.Property(e => e.Price)
                    .IsRequired()
                    .HasColumnType("decimal(18,2)");

                entity.Property(e => e.StockQuantity)
                    .IsRequired();

                entity.Property(e => e.Category)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("GETDATE()");

                entity.Property(e => e.UpdatedAt)
                    .HasDefaultValueSql("GETDATE()");

                // Unique indexes
                entity.HasIndex(e => e.Name)
                    .IsUnique()
                    .HasDatabaseName("IX_Products_Name_Unique");

                entity.HasIndex(e => e.Sku)
                    .IsUnique()
                    .HasDatabaseName("IX_Products_Sku_Unique");
            });

            // ==================== Order Configuration ====================
            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("Orders");

                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.ProductId)
                    .IsRequired();

                entity.Property(e => e.OrderNumber)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.CustomerName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.CustomerEmail)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.Quantity)
                    .IsRequired();

                entity.Property(e => e.OrderDate)
                    .IsRequired();

                entity.Property(e => e.DeliveryDate);

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("GETDATE()");

                entity.Property(e => e.UpdatedAt)
                    .HasDefaultValueSql("GETDATE()");

                // Unique indexes
                entity.HasIndex(e => e.OrderNumber)
                    .IsUnique()
                    .HasDatabaseName("IX_Orders_OrderNumber_Unique");

                entity.HasIndex(e => e.CustomerEmail)
                    .IsUnique()
                    .HasDatabaseName("IX_Orders_CustomerEmail_Unique");

                // Relationship
                entity.HasOne(o => o.Product)
                      .WithMany(p => p.Orders)
                      .HasForeignKey(o => o.ProductId)
                      .OnDelete(DeleteBehavior.Restrict)   // Không cho xóa sản phẩm nếu còn đơn hàng
                      .HasConstraintName("FK_Orders_Products");
            });

            // ==================== Seeding ====================
            SeedProducts(modelBuilder);
            SeedOrders(modelBuilder);
        }

        private static void SeedProducts(ModelBuilder modelBuilder)
        {
            var products = new[]
            {
                new Product { Name = "iPhone 15 Pro",        Sku = "IPH15P",   Description = "Apple flagship", Price = 999.99m,  StockQuantity = 50,  Category = "Smartphone" },
                new Product { Name = "Galaxy S24 Ultra",     Sku = "S24U",     Description = "Samsung top model", Price = 1299.99m, StockQuantity = 40,  Category = "Smartphone" },
                new Product { Name = "MacBook Air M3",       Sku = "MBA-M3",   Description = "Apple laptop", Price = 1099.99m,  StockQuantity = 30,  Category = "Laptop" },
                new Product { Name = "Dell XPS 14",          Sku = "XPS14",    Description = "Premium ultrabook", Price = 1499.99m, StockQuantity = 25,  Category = "Laptop" },
                new Product { Name = "Sony WH-1000XM5",      Sku = "WHXM5",    Description = "Noise cancelling", Price = 349.99m,  StockQuantity = 120, Category = "Headphone" },
                new Product { Name = "AirPods Pro 2",        Sku = "APP2",     Description = "Apple earbuds", Price = 249.99m,   StockQuantity = 200, Category = "Headphone" },
                new Product { Name = "Logitech MX Master 3S",Sku = "MX3S",     Description = "Ergonomic mouse", Price = 99.99m,   StockQuantity = 150, Category = "Accessories" },
                new Product { Name = "Samsung 55\" QN90C",   Sku = "QN90C55",  Description = "Neo QLED TV", Price = 1499.99m,   StockQuantity = 18,  Category = "TV" },
                new Product { Name = "LG OLED C3 65\"",      Sku = "OLED65C3", Description = "OLED television", Price = 2199.99m, StockQuantity = 12,  Category = "TV" },
                new Product { Name = "Canon EOS R6 Mark II", Sku = "R6M2",     Description = "Mirrorless camera", Price = 2499.99m, StockQuantity = 15,  Category = "Camera" },
                new Product { Name = "Nikon Z6 III",         Sku = "Z6III",    Description = "Full-frame mirrorless", Price = 2499.99m, StockQuantity = 10, Category = "Camera" },
                new Product { Name = "Samsung T7 1TB SSD",   Sku = "T7-1TB",   Description = "Portable SSD", Price = 109.99m,    StockQuantity = 300, Category = "Storage" },
                new Product { Name = "WD Black SN850X 2TB",  Sku = "SN850X2TB",Description = "NVMe SSD", Price = 169.99m,     StockQuantity = 80,  Category = "Storage" },
                new Product { Name = "Microsoft Surface Pro 9", Sku = "SP9",   Description = "2-in-1 tablet", Price = 999.99m,   StockQuantity = 35,  Category = "Tablet" },
                new Product { Name = "iPad Pro 12.9\" M2",   Sku = "IPDPRO129",Description = "Apple tablet", Price = 1299.99m,  StockQuantity = 45,  Category = "Tablet" }
            };

            for (int i = 0; i < products.Length; i++)
            {
                products[i].CreatedAt = DateTime.UtcNow;
                products[i].UpdatedAt = DateTime.UtcNow;
            }

            modelBuilder.Entity<Product>().HasData(products);
        }

        private static void SeedOrders(ModelBuilder modelBuilder)
        {
            var random = new Random(2025); // seed cố định để dễ test lại

            var orderNumbers = Enumerable.Range(1, 30)
                .Select(i => $"ORD-{DateTime.UtcNow:yyyyMMdd}-{i:0000}")
                .ToArray();

            var customerNames = new[] { "Nguyễn Văn A", "Trần Thị B", "Lê Văn C", "Phạm Thị D", "Hoàng Văn E", "Vũ Thị F", "Đặng Văn G" };
            var emails = new[] { "a@gmail.com", "b@yahoo.com", "c@outlook.com", "d@hotmail.com", "e@zoho.com", "f@proton.me", "g@icloud.com" };

            var orders = new List<Order>();

            for (int i = 0; i < 30; i++)
            {
                var productId = random.Next(1, 16); // 1..15
                var quantity = random.Next(1, 6);
                var orderDate = DateTime.UtcNow.AddDays(-random.Next(1, 45));
                var deliveryDate = random.Next(0, 3) == 0 ? orderDate.AddDays(random.Next(2, 10)) : (DateTime?)null;

                orders.Add(new Order
                {
                    ProductId = productId,
                    OrderNumber = orderNumbers[i],
                    CustomerName = customerNames[random.Next(customerNames.Length)],
                    CustomerEmail = emails[random.Next(emails.Length)],
                    Quantity = quantity,
                    OrderDate = orderDate,
                    DeliveryDate = deliveryDate,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }

            modelBuilder.Entity<Order>().HasData(orders);
        }
    }
}