using System.ComponentModel.DataAnnotations;
using IdentityService.Models;
using Microsoft.AspNetCore.Identity;

namespace IdentityService.Data;

public static class DbSeeder
{
    [DataType(DataType.Password)]
    static string DefaultPassword = "Password123!";
    public static async Task SeedDatabaseAsync(
        IApplicationBuilder app
        )
    {
        using var scope = app.ApplicationServices.CreateScope();
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<Program>>();

        try
        {

            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

            logger.LogInformation("Starting database seeding...");

            await SeedRolesAsync(roleManager, logger);

            await SeedRoleUserAsync(userManager, logger);

            logger.LogInformation("Database seeding completed.");

        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred during database seeding.");
        }
    }

    public static async Task SeedRolesAsync
    (
        RoleManager<IdentityRole> roleManager,
        ILogger logger
    )
    {
        try
        {
            var roles = new[] { "Admin", "User", "ProductManager", "OrderManager", "CustomerSupport", "DeliveryDriver", "DeliveryManager", "DeliverySupport" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    var result = await roleManager.CreateAsync(new IdentityRole(role));
                    if (result.Succeeded)
                    {
                        logger.LogInformation($"Role '{role}' created successfully.");
                    }
                    else
                    {
                        logger.LogError($"Error creating role '{role}': {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    }
                }
                else
                {
                    logger.LogInformation($"Role '{role}' already exists.");
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding roles.");
        }
    }

    public static async Task SeedRoleUserAsync
    (
        UserManager<ApplicationUser> userManager,
        ILogger logger
    )
    {
        try
        {
            string[] userRoles = { "Admin", "User", "ProductManager" };

            foreach (string userRole in userRoles)
            {
                var userEmail = $"{userRole.ToLower()}@example.com";
                var user = await userManager.FindByEmailAsync(userEmail);
                if (user != null)
                {
                    logger.LogInformation($"User for role '{userRole}' already exists.");

                    await AssignRoleAsync(userManager, user, userRole, logger);
                }
                else
                {
                    ApplicationUser newUser = new ApplicationUser
                    {
                        UserName = userEmail,
                        Email = userEmail,
                        EmailConfirmed = true

                    };

                    //string defaultPassword = "Password123!";

                    var result = await userManager.CreateAsync(newUser, DefaultPassword);
                    if (result.Succeeded)
                    {
                        await AssignRoleAsync(userManager, newUser, userRole, logger);
                        logger.LogWarning("User with email {UserEmail} created. And assigned the {UserRole} role", userEmail, userRole);
                    }
                    else
                    {
                        logger.LogError("Failed to create user {UserEmail}: {Errors}", userEmail, string.Join(", ", result.Errors.Select(e => e.Description)));

                        await AssignRoleAsync(userManager, newUser, userRole, logger);

                        logger.LogWarning("User with email {UserEmail} created. And assigned the {UserRole} role", userEmail, userRole);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding admin users.");
        }
    }

    public static async Task AssignRoleAsync(
        UserManager<ApplicationUser> userManager,
        ApplicationUser user,
        string role,
        ILogger logger
    )
    {
        try
        {
            if (!await userManager.IsInRoleAsync(user, role))
            {
                await userManager.AddToRoleAsync(user, role);
                logger.LogInformation("Assigned role {Role} to user {UserEmail}", role, user.Email);
            }
            else
            {
                logger.LogInformation("User {UserEmail} already has role {Role}", user.Email, role);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while assigning role {Role} to user {UserEmail}", role, user.Email);
        }
    }

}