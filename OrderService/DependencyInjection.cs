using IdentityModel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;

namespace OrderService;

public static class DependencyInjection
{
    public static IServiceCollection AddOrderServices(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment
        )
    {

        // 1. Add Swagger
        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(options =>
        {

            options.SwaggerDoc("v1", new OpenApiInfo { Title = "OrderService API", Version = "v1" });

            var securityScheme = new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "JWT Authorization header using the Bearer scheme."
            };

            options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, securityScheme);

            var securityRequirement = new OpenApiSecurityRequirement
            {
                { securityScheme, new[] { JwtBearerDefaults.AuthenticationScheme } }
            };

            options.AddSecurityRequirement(securityRequirement);

        });

        // 2. Add Authentication and Authorization
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = "https://localhost:7000";
                options.RequireHttpsMetadata = environment.IsProduction();
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


        // 3. Add Authorization policies
        services.AddAuthorization(options =>
        {
            options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
            options.AddPolicy("OrderManager", policy => policy.RequireRole("OrderManager"));
            options.AddPolicy("AdminOrOrderManager", policy => policy.RequireRole("Admin", "OrderManager"));
            options.AddPolicy("User", policy => policy.RequireRole("User"));
        });


        // 4. Database connection
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        return services;
    }
}