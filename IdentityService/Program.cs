using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddDatabaseAndIdentity(builder.Configuration);

builder.Services.AddIdentityServices();

builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "Enterprise E-Commerce - Identity Service API (.NET 8)",
        Version = "v1",
        Description = "Manages user authentication, registration, and authorization for the entire platform. Built with .NET 8."
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "IdentityService API v1");
        options.RoutePrefix = string.Empty; // Set Swagger UI at the app's root
    });
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseIdentityServer();

app.UseAuthorization();

app.MapIdentityApiEndpoints();

app.MapGet("/health", () =>
{
    return Results.Ok(new { status = "Healthy", timestamp = DateTime.UtcNow });
})
.WithName("GetHealthCheck")
.WithTags("Health");

app.Run();
