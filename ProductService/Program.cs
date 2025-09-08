
using ProductService;
using ProductService.Data;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProductServiceDependencies(builder.Configuration, builder.Environment);

var app = builder.Build();

app.UseAuthentication();

app.UseAuthorization();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Product API v1");
    });
}

app.UseAuthentication();

app.UseAuthorization();

app.MapProductApiEndpoints();

// --- Database Seeding --------------------------------------------------------
//DbSeeder.SeedDatabase(app);

app.Run();

