using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDependencyInjectionServices(builder.Configuration);

var app = builder.Build();

app.MapIdentityApiEndpoints();

        app.MapGet("/health", () =>
        {
            return Results.Ok(new { status = "Healthy", timestamp = DateTime.UtcNow });
        })
        .WithName("GetHealthCheck")
        .WithTags("Health");

DependencyInjection.UseDependencyInjection(app, app.Environment);

app.Run();
