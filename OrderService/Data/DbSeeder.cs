using Microsoft.EntityFrameworkCore;
using OrderService.Models;

namespace OrderService.Data;

public static class DbSeeder
{
    public static void SeedDatabase(IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<OrderDataBaseContext>();

        Console.WriteLine("--> Attempting to apply migrations...");
        context.Database.Migrate();

        if (context.Orders.Any())
        {
            Console.WriteLine("--> Database already seeded.");
            return;
        }

        Console.WriteLine("--> Seeding realistic sample order data...");

        var ordersToSeed = new List<Order>();

        // --- Order 1 (Praboth) ---
        var order1Items = new List<OrderItem>
        {
            new() { ProductId = "1", ProductName = "Quantum Laptop", UnitPrice = 1499.99m, Quantity = 1, Subtotal = 1499.99m },
            new() { ProductId = "4", ProductName = "Ergonomic Mouse", UnitPrice = 79.00m, Quantity = 1, Subtotal = 79.00m },
        };
        ordersToSeed.Add(new Order
        {
            CustomerId = "bdb292a2-32fe-4d16-8fad-07c35a5b0ef7",
            CustomerName = "Praboth@example.com",
            TotalAmount = order1Items.Sum(oi => oi.Subtotal),
            IsPaid = true,
            PaidAt = DateTime.UtcNow.AddDays(-1),
            OrderItems = order1Items
        });
        
        // --- Order 2 (user1) ---
        var order2Items = new List<OrderItem>
        {
            new() { ProductId = "15", ProductName = "Gaming Console", UnitPrice = 499.99m, Quantity = 1, Subtotal = 499.99m }
        };
        ordersToSeed.Add(new Order
        {
            CustomerId = "8563be63-7221-4d77-91ac-935ca00accdf",
            CustomerName = "user1@example.com",
            TotalAmount = order2Items.Sum(oi => oi.Subtotal),
            IsPaid = true,
            PaidAt = DateTime.UtcNow.AddDays(-2),
            OrderItems = order2Items
        });
        
        // --- Order 3 (user) ---
        var order3Items = new List<OrderItem>
        {
            new() { ProductId = "5", ProductName = "Noise-Cancelling Headphones", UnitPrice = 199.99m, Quantity = 2, Subtotal = 399.98m }
        };
        ordersToSeed.Add(new Order
        {
            CustomerId = "a9b7ec7f-4846-4ff1-b7de-b22de384f6d3",
            CustomerName = "user@example.com",
            TotalAmount = order3Items.Sum(oi => oi.Subtotal),
            IsPaid = false, // Unpaid order
            PaidAt = null,
            OrderItems = order3Items
        });
        
        // --- Order 4 (Praboth) ---
        var order4Items = new List<OrderItem>
        {
            new() { ProductId = "11", ProductName = "VR Headset", UnitPrice = 399.99m, Quantity = 1, Subtotal = 399.99m },
            new() { ProductId = "10", ProductName = "Action Camera", UnitPrice = 299.99m, Quantity = 1, Subtotal = 299.99m }
        };
        ordersToSeed.Add(new Order
        {
            CustomerId = "bdb292a2-32fe-4d16-8fad-07c35a5b0ef7",
            CustomerName = "Praboth@example.com",
            TotalAmount = order4Items.Sum(oi => oi.Subtotal),
            IsPaid = true,
            PaidAt = DateTime.UtcNow.AddDays(-5),
            OrderItems = order4Items
        });
        
        // --- Order 5 (user1) ---
        var order5Items = new List<OrderItem>
        {
            new() { ProductId = "8", ProductName = "Wireless Charger", UnitPrice = 39.99m, Quantity = 5, Subtotal = 199.95m }
        };
        ordersToSeed.Add(new Order
        {
            CustomerId = "8563be63-7221-4d77-91ac-935ca00accdf",
            CustomerName = "user1@example.com",
            TotalAmount = order5Items.Sum(oi => oi.Subtotal),
            IsPaid = false, // Unpaid order
            PaidAt = null,
            OrderItems = order5Items
        });
        
        // --- Order 6 (user) ---
        var order6Items = new List<OrderItem>
        {
            new() { ProductId = "14", ProductName = "E-Reader", UnitPrice = 119.99m, Quantity = 1, Subtotal = 119.99m },
        };
        ordersToSeed.Add(new Order
        {
            CustomerId = "a9b7ec7f-4846-4ff1-b7de-b22de384f6d3",
            CustomerName = "user@example.com",
            TotalAmount = order6Items.Sum(oi => oi.Subtotal),
            IsPaid = true,
            PaidAt = DateTime.UtcNow.AddDays(-10),
            OrderItems = order6Items
        });

        // --- Order 7 (user1) ---
        var order7Items = new List<OrderItem>
        {
            new() { ProductId = "6", ProductName = "Portable SSD", UnitPrice = 159.99m, Quantity = 1, Subtotal = 159.99m },
            new() { ProductId = "7", ProductName = "Smartwatch", UnitPrice = 249.99m, Quantity = 1, Subtotal = 249.99m }
        };
        ordersToSeed.Add(new Order
        {
            CustomerId = "8563be63-7221-4d77-91ac-935ca00accdf",
            CustomerName = "user1@example.com",
            TotalAmount = order7Items.Sum(oi => oi.Subtotal),
            IsPaid = true,
            PaidAt = DateTime.UtcNow.AddHours(-3),
            OrderItems = order7Items
        });
        
        // --- Order 8 (Praboth) ---
        var order8Items = new List<OrderItem>
        {
            new() { ProductId = "13", ProductName = "Fitness Tracker", UnitPrice = 99.99m, Quantity = 3, Subtotal = 299.97m },
        };
        ordersToSeed.Add(new Order
        {
            CustomerId = "bdb292a2-32fe-4d16-8fad-07c35a5b0ef7",
            CustomerName = "Praboth@example.com",
            TotalAmount = order8Items.Sum(oi => oi.Subtotal),
            IsPaid = false, // Unpaid order
            PaidAt = null,
            OrderItems = order8Items
        });
        
        context.Orders.AddRange(ordersToSeed);
        context.SaveChanges();
    }
}

