using IdentityService.Data;
using IdentityService.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

public static class DependencyInjection
{

    /// <summary>
    /// Registers Duende IdentityServer and its in-memory stores for clients and scopes.
    /// </summary>
    /// <param name="services">The IServiceCollection to add the services to.</param>
    /// <returns>The configured IServiceCollection.</returns>
    public static IServiceCollection AddIdentityServices(this IServiceCollection services)
    {
        services.AddIdentityServer(options =>
        {
            options.LicenseKey = null; // Set to null for development purposes
            options.KeyManagement.Enabled = false; // Disable key management for simplicity in development
        })
            .AddInMemoryApiScopes(IdentityServerConfig.GetApiScopes())
            .AddInMemoryClients(IdentityServerConfig.GetClients())
            .AddAspNetIdentity<ApplicationUser>()
            .AddDeveloperSigningCredential(); // Use a developer signing credential for simplicity in development

        return services;
    }


    /// <summary>
    /// Registers the database context and ASP.NET Core Identity services.
    /// </summary>
    public static IServiceCollection AddDatabaseAndIdentity(this IServiceCollection services, IConfiguration configuration)
    {
        // Configure the database context to use PostgreSQL.
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString));

        // Configure ASP.NET Core Identity.
        services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        return services;
    }
}