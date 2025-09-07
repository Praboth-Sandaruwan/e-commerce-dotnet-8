using Microsoft.OpenApi.Models;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        // will go to get the public key to validate the token's signature.
        options.Authority = "https://localhost:7000"; // TODO: Move to configuration

        options.TokenValidationParameters = new TokenValidationParameters
        {
            // The "aud" (audience) claim in the token must match this value.
            ValidateAudience = false, // In a real app, you might validate this.
            ValidIssuer = "https://localhost:7000"
        };

        // For development with self-signed certs, the ProductService needs to
        // trust the IdentityService's certificate. This handler bypasses that
        // validation. DO NOT USE THIS IN PRODUCTION.
        if (builder.Environment.IsDevelopment())
        {
            options.BackchannelHttpHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };
        }
    });

builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new() { Title = "E-Commerce Product Service API (.NET 8)", Version = "v1" });

    // Add a security definition for Bearer tokens
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });

    // Make sure Swagger UI requires a Bearer token to access endpoints
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[]{}
        }
    });

});

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

app.MapGet("/health", () => Results.Ok(new { status = "Healthy" }))
   .WithName("HealthCheck")
   .WithTags("Testing");


app.MapGet("/api/products", () => Results.Ok(new List<object>
    {
        new { Id = 1, Name = "Laptop" },
        new { Id = 2, Name = "Mouse" }
    }))
    .RequireAuthorization() // This endpoint now requires a valid token (for testing)
    .WithName("GetProductsTest")
    .WithTags("Testing");

app.Run();

