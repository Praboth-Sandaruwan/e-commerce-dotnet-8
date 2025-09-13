using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using ProductService.Data;

namespace ProductService;

public static class DependencyInjection
{
    public static IServiceCollection AddProductServiceDependencies(
        this IServiceCollection services, 
        IConfiguration configuration, 
        IWebHostEnvironment environment)
    {
        // 1. Add API Documentation (Swagger)
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please enter a valid token",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = "Bearer"
            });
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

        // 2. Add Authentication and Authorization
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = "https://localhost:7000";
                options.RequireHttpsMetadata = true;
                options.TokenValidationParameters.ValidIssuer = "https://localhost:7000";
                options.TokenValidationParameters.ValidateAudience = false;

                if (environment.IsDevelopment())
                {
                    options.BackchannelHttpHandler = new HttpClientHandler
                    {
                        ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                    };
                }
            });
            
        services.AddAuthorization(options =>
        {
            options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
            options.AddPolicy("ProductManager", policy => policy.RequireRole("ProductManager"));
            options.AddPolicy("AdminOrProductManager", policy => policy.RequireRole("Admin", "ProductManager"));
            options.AddPolicy("User", policy => policy.RequireRole("User"));
        });

        // 3. Add Database Context
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<ProductDatabaseContext>(options =>
            options.UseNpgsql(connectionString));

        return services;
    }
}