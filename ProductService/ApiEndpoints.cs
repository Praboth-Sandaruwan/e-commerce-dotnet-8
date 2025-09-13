using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ProductService.Data;
using ProductService.Models;
using ProductService.Models.DTOs;

namespace ProductService;

public static class ApiEndpoints
{
    public static void MapProductApiEndpoints(this IEndpointRouteBuilder app)
    {
        // request params : none   response: 200 : products list, 401, 403
        // public endpoint
        app.MapGet("/health", () => Results.Ok("Product Service is healthy."))
            .WithName("HealthCheck")
            .WithTags("Health");

        app.MapGet("/api/products", async (ProductDatabaseContext db) =>
        {
            var products = await db.Products.ToListAsync();
            return Results.Ok(products);
        })
        .WithName("GetProducts")
        .WithTags("Products");

        // request params : id   response: 200 : product, 400, 401, 403, 404
        // public endpoint
        app.MapGet("/api/products/{id}", async (
            ProductDatabaseContext db,
            [FromRoute] string id) =>
        {
            try
            {
                var productId = int.Parse(id);
                var product = await db.Products.FindAsync(productId);
                return product is not null ? Results.Ok(
                    new ProductDto
                    {
                        Id = product.Id,
                        Name = product.Name,
                        Description = product.Description,
                        Price = product.Price,
                        Category = product.Category,
                        StockQuantity = product.StockQuantity,
                        CreatedAt = product.CreatedAt
                    }
                ) : Results.NotFound();
            }
            catch (FormatException)
            {
                return Results.BadRequest("Invalid product ID format.");
            }
        })
        .WithName("GetProductById")
        .WithTags("Products");

        // request params : product in body   response: 201 : product with Id, 400, 401, 403
        // protected endpoint
        app.MapPost("/api/products", async (
            ProductDatabaseContext db,
            [FromBody] ProductCreateDto model) =>
        {
            try
            {
                if (model is null)
                {
                    throw new ArgumentNullException(nameof(model), $"Product data is required : {model}");
                }

                Product product = new Product
                {
                    Name = model.Name,
                    Description = model.Description,
                    Price = model.Price,
                    Category = model.Category,
                    StockQuantity = model.StockQuantity,
                    CreatedAt = model.CreatedAt
                };

                db.Products.Add(product);
                await db.SaveChangesAsync();
                return Results.Created($"/api/products/{product.Id}", product);
            }
            catch (DbUpdateException ex)
            {
                return Results.BadRequest($"Database update error: {ex.Message}");
            }
            catch (ArgumentNullException ex)
            {
                return Results.BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return Results.BadRequest($"An error occurred: {ex.Message}");
            }

        })
        .WithName("CreateProduct")
        .WithTags("Products")
        .RequireAuthorization("AdminOrProductManager");

        // request params : id, product in body   response: 201 : updated product, 400, 401, 403, 404
        // protected endpoint
        app.MapPut("/api/products/{id}", async (
            ProductDatabaseContext db,
            [FromRoute] string id,
            [FromBody] ProductUpdateDto model) =>
        {
            try
            {
                var productId = int.Parse(id);
                var existingProduct = await db.Products.FindAsync(productId);
                if (existingProduct is null)
                {
                    return Results.NotFound($"Product with ID {productId} not found.");
                }

                existingProduct.Name = model.Name.IsNullOrEmpty() ? existingProduct.Name : model.Name;
                existingProduct.Description = model.Description.IsNullOrEmpty() ? existingProduct.Description : model.Description;
                existingProduct.Price = model.Price != 0 ? model.Price : existingProduct.Price;
                existingProduct.Category = model.Category.IsNullOrEmpty() ? existingProduct.Category : model.Category;
                existingProduct.StockQuantity = model.StockQuantity != 0 ? model.StockQuantity : existingProduct.StockQuantity;

                await db.SaveChangesAsync();
                return Results.Created($"/api/products/{existingProduct.Id}", existingProduct);
            }
            catch (FormatException)
            {
                return Results.BadRequest("Invalid product ID format.");
            }
            catch (DbUpdateException ex)
            {
                return Results.BadRequest($"Database update error: {ex.Message}");
            }
            catch (Exception ex)
            {
                return Results.BadRequest($"An error occurred: {ex.Message}");
            }
        })
        .WithName("UpdateProduct")
        .WithTags("Products")
        .RequireAuthorization("AdminOrProductManager");

        // request params : id   response: 204 : no content, 400, 401, 403, 404
        // protected endpoint
        app.MapDelete("/api/products/{id}", async (
            ProductDatabaseContext db,
            [FromRoute] string id) =>
        {
            try
            {
                var productId = int.Parse(id);
                var existingProduct = await db.Products.FindAsync(productId);
                if (existingProduct is null)
                {
                    return Results.NotFound($"Product with ID {productId} not found.");
                }

                db.Products.Remove(existingProduct);
                await db.SaveChangesAsync();
                return Results.NoContent();
            }
            catch (FormatException)
            {
                return Results.BadRequest("Invalid product ID format.");
            }
            catch (DbUpdateException ex)
            {
                return Results.BadRequest($"Database update error: {ex.Message}");
            }
            catch (Exception ex)
            {
                return Results.BadRequest($"An error occurred: {ex.Message}");
            }
        })
        .WithName("DeleteProduct")
        .WithTags("Products")
        .RequireAuthorization("AdminOrProductManager");

        // request params : id, quantity   response: 200 : updated product stock, 400, 401, 403, 404
        // protected endpoint
        app.MapPost("/api/products/{id}", async (
            ProductDatabaseContext db,
            [FromRoute] string id,
            [FromQuery] string operation,
            [FromBody] ProductStockUpdateDto model) =>
        {

            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    throw new ArgumentNullException(nameof(id), "Product ID is required.");
                }

                if (string.IsNullOrWhiteSpace(operation) ||
                    (operation != "add" && operation != "subtract"))
                {
                    throw new ArgumentException("Operation must be either 'add' or 'subtract'.", nameof(operation));
                }

                if (!int.TryParse(id, out int productId) || productId <= 0)
                {
                    throw new ArgumentException("Invalid product ID format.", nameof(id));
                }

                var existingProduct = await db.Products.FindAsync(productId);

                if (existingProduct is null)
                {
                    return Results.NotFound($"Product with ID {productId} not found.");
                }

                if (operation == "add")
                {
                    existingProduct.StockQuantity += model.StockQuantity;
                }
                else if (operation == "subtract")
                {
                    if (existingProduct.StockQuantity < model.StockQuantity)
                    {
                        return Results.BadRequest("Insufficient stock to subtract the specified quantity.");
                    }
                    existingProduct.StockQuantity -= model.StockQuantity;
                }

                await db.SaveChangesAsync();
                return Results.Ok(existingProduct);

            }
            catch (ArgumentNullException ex)
            {
                return Results.BadRequest(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(ex.Message);
            }
            catch (DbUpdateException ex)
            {
                return Results.BadRequest($"Database update error: {ex.Message}");
            }
            catch (Exception ex)
            {
                return Results.BadRequest($"An error occurred: {ex.Message}");
            }

        })
        .WithName("UpdateProductStock")
        .WithTags("Products")
        .RequireAuthorization("AdminOrProductManager");
    }
}