using Microsoft.EntityFrameworkCore;
using ProductService.Data;

namespace ProductService;

public static class ApiEndpoints
{
    public static void MapProductApiEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/health", () => Results.Ok("Product Service is healthy."))
            .WithName("HealthCheck")
            .WithTags("Health");

        app.MapGet("/api/products", async (ProductDatabaseContext db) =>
        {
            var products = await db.Products.ToListAsync();
            return Results.Ok(products);
        })
        .RequireAuthorization() // This endpoint remains protected
        .WithName("GetProducts")
        .WithTags("Products");
    }
}