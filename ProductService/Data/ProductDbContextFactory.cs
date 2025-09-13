using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using ProductService.Data;
using System.IO;
using Npgsql.EntityFrameworkCore.PostgreSQL; 

public class ProductDbContextFactory : IDesignTimeDbContextFactory<ProductDatabaseContext>
{
    public ProductDatabaseContext CreateDbContext(string[] args)
    {
        // This builds a configuration object that reads from appsettings.json,
        // just like the main application does.
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.Development.json")
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<ProductDatabaseContext>();
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        optionsBuilder.UseNpgsql(connectionString);

        return new ProductDatabaseContext(optionsBuilder.Options);
    }
}