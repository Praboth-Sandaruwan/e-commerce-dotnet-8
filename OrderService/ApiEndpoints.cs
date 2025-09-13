using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using OrderService.Data;
using OrderService.Dto;
using OrderService.Models;

namespace OrderService;

public static class ApiEndpoints
{
    public static void MapOrderApiEndpoints(this IEndpointRouteBuilder app)
    {
        var orders = app.MapGroup("/api/orders").RequireAuthorization();

        // GET /api/orders
        // Admin Only: Gets a list of all orders in the system.
        orders.MapGet("/", async (OrderDataBaseContext db) =>
        {
            var allOrders = await db.Orders
                .Include(o => o.OrderItems)
                .AsNoTracking()
                .Select(o => OrderResponseMapper.MapToOrderResponseDto(o))
                .ToListAsync();

            return Results.Ok(allOrders);
        })
        .RequireAuthorization("AdminOrOrderManager")
        .WithName("GetAllOrders")
        .WithTags("Orders");

        // GET /api/orders/my-orders
        // Customer: Gets a list of orders for the currently authenticated user.
        orders.MapGet("/my-orders", async (OrderDataBaseContext db, HttpContext http) =>
        {
            var userId = http.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Results.Unauthorized();
            }

            var customerOrders = await db.Orders
                .Where(o => o.CustomerId == userId)
                .Include(o => o.OrderItems)
                .AsNoTracking()
                .Select(o => OrderResponseMapper.MapToOrderResponseDto(o))
                .ToListAsync();

            return Results.Ok(customerOrders);
        })
        .RequireAuthorization("AdminOrOrderManagerOrUser")
        .WithName("GetMyOrders")
        .WithTags("Orders");

        // GET /api/orders/{id}
        // Gets a specific order by its ID.
        // An admin can get any order. A customer can only get their own.
        orders.MapGet("/{id:guid}", async (Guid id, OrderDataBaseContext db, HttpContext http) =>
        {
            var order = await db.Orders
                .Include(o => o.OrderItems)
                .AsNoTracking()
                .Select(o => OrderResponseMapper.MapToOrderResponseDto(o))
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order is null)
            {
                return Results.NotFound();
            }

            var userId = http.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isRegisteredUser = http.User.IsInRole("User") || http.User.IsInRole("OrderManager") || http.User.IsInRole("Admin");
            var isOrderManagerOrAdmin = http.User.IsInRole("OrderManager") || http.User.IsInRole("Admin");

            // If the user is an OrderManager or Admin, they can access any order.
            if (isOrderManagerOrAdmin)
            {
                return Results.Ok(order);
            }

            // If the user is not an Registered user and the order does not belong to them, forbid access.
            if (!isRegisteredUser && order.CustomerId != userId)
            {
                return Results.Forbid();
            }

            return Results.Ok(order);
        })
        .RequireAuthorization("AdminOrOrderManagerOrUser")
        .WithName("GetMyOrderById")
        .WithTags("Orders");

        // POST /api/orders
        // Customer: Creates a new order.
        orders.MapPost("/", async (
            CreateOrderDto orderModel,
            OrderDataBaseContext db,
            HttpContext http) =>
        {
            var userId = http.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userName = http.User.FindFirstValue(ClaimTypes.Name) ?? "Unknown";

            if (string.IsNullOrEmpty(userId))
            {
                return Results.Unauthorized();
            }

            try
            {
                var orderItems = orderModel.Items.Select(item => new OrderItem
                {
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    UnitPrice = item.UnitPrice,
                    Quantity = item.Quantity,
                    Subtotal = item.UnitPrice * item.Quantity
                }).ToList();

                var newOrder = new Order
                {
                    CustomerId = userId,
                    CustomerName = userName,
                    OrderItems = orderItems,
                    TotalAmount = orderItems.Sum(item => item.Subtotal),
                    IsPaid = false,
                    PaidAt = null
                };

                await db.Orders.AddAsync(newOrder);
                await db.SaveChangesAsync();

                var createdOrder = OrderResponseMapper.MapToOrderResponseDto(newOrder);
                return Results.CreatedAtRoute($"/api/orders/{createdOrder.Id}", new { id = createdOrder.Id }, createdOrder);
            }
            catch (Exception ex)
            {
                return Results.Problem($"An error occurred while creating the order: {ex.Message}");
            }
        })
        .RequireAuthorization("AdminOrOrderManagerOrUser")
        .WithName("CreateOrder")
        .WithTags("Orders");

        

    }
}

