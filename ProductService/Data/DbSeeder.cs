using Microsoft.EntityFrameworkCore;
using ProductService.Models;

namespace ProductService.Data;

public static class DbSeeder
{
    public static void SeedDatabase(IApplicationBuilder app)
    {
        // We get a service scope to resolve the DbContext. This is the correct
        // way to access scoped services (like a DbContext) in startup code.
        using var scope = app.ApplicationServices.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ProductDatabaseContext>();

        // Apply any pending migrations to ensure the database is up-to-date.
        dbContext.Database.Migrate();

        // If the Products table is already populated, do nothing.
        if (dbContext.Products.Any())
        {
            Console.WriteLine("--> Database already seeded.");
            return;
        }

        Console.WriteLine("--> Seeding database with sample products...");

        var products = new List<Product>
        {
            new Product { Name = "Quantum Laptop", Description = "A high-performance laptop for developers.", Price = 1499.99m, StockQuantity = 50 },
            new Product { Name = "Mechanical Keyboard", Description = "RGB mechanical keyboard with cherry MX switches.", Price = 129.99m, StockQuantity = 200 },
            new Product { Name = "4K IPS Monitor", Description = "A 27-inch 4K monitor with excellent color accuracy.", Price = 499.50m, StockQuantity = 150 },
            new Product { Name = "Ergonomic Mouse", Description = "A wireless ergonomic mouse to reduce wrist strain.", Price = 79.00m, StockQuantity = 300 },
            new Product { Name = "Noise-Cancelling Headphones", Description = "Over-ear headphones with active noise cancellation.", Price = 199.99m, StockQuantity = 100 },
            new Product { Name = "Portable SSD", Description = "1TB portable SSD with USB-C connectivity.", Price = 159.99m, StockQuantity = 250 },
            new Product { Name = "Smartwatch", Description = "A smartwatch with fitness tracking and notifications.", Price = 249.99m, StockQuantity = 120 },
            new Product { Name = "Wireless Charger", Description = "Fast wireless charger compatible with all Qi devices.", Price = 39.99m, StockQuantity = 400 },
            new Product { Name = "Bluetooth Speaker", Description = "Portable Bluetooth speaker with deep bass.", Price = 89.99m, StockQuantity = 180 },
            new Product { Name = "Action Camera", Description = "4K action camera with waterproof housing.", Price = 299.99m, StockQuantity = 80 },
            new Product { Name = "VR Headset", Description = "Immersive VR headset for gaming and simulations.", Price = 399.99m, StockQuantity = 60 },
            new Product { Name = "Smart Home Hub", Description = "Control all your smart home devices from one hub.", Price = 129.99m, StockQuantity = 140 },
            new Product { Name = "Fitness Tracker", Description = "Track your daily activity and health metrics.", Price = 99.99m, StockQuantity = 220 },
            new Product { Name = "E-Reader", Description = "Lightweight e-reader with a high-resolution display.", Price = 119.99m, StockQuantity = 160 },
            new Product { Name = "Gaming Console", Description = "Next-gen gaming console with stunning graphics.", Price = 499.99m, StockQuantity = 70 }
        };

        dbContext.Products.AddRange(products);
        dbContext.SaveChanges();
    }
}