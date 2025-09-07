using System.Security.Claims;
using Duende.IdentityServer;
using Duende.IdentityServer.Services;
using IdentityModel;
using IdentityService.Data;
using IdentityService.Models;
using IdentityService.Models.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public static class ApiEndpoints
{
    public static IEndpointRouteBuilder MapIdentityApiEndpoints(this IEndpointRouteBuilder app)
    {

        var accountGroup = app.MapGroup("/api/account").WithTags("Account");



        /* Register a new user */
        accountGroup.MapPost("/register", async (
            [FromBody] RegisterRequestDto model,
            [FromServices] UserManager<ApplicationUser> userManager
        ) =>
        {
            if (model == null || !model.Password.Equals(model.ConfirmPassword))
            {
                return Results.BadRequest(new { message = "Invalid registration data." });
            }

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                EmailConfirmed = true // In a real application, consider sending a confirmation email instead.
            };

            var result = await userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                return Results.Created($"/api/account/{user.Id}", new { Message = "User registered successfully." });
            }

            var errors = result.Errors.Select(e => e.Description);
            return Results.BadRequest(new { Errors = errors });

        })
        .Produces(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest)
        .WithName("RegisterUser");



        /* Login an existing user */
        accountGroup.MapPost("/login", async (
            [FromBody] LoginRequestDto model,
            [FromServices] SignInManager<ApplicationUser> signInManager,
            [FromServices] UserManager<ApplicationUser> userManager,
            HttpContext httpContext,
            ApplicationDbContext dbContext,
            [FromServices] IIdentityServerTools identityServerTools) =>
        {
            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return Results.Unauthorized();
            }

            var result = await signInManager.CheckPasswordSignInAsync(user, model.Password, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                // --- Access Token Generation ---
                var claims = new List<Claim>
            {
                new(JwtClaimTypes.Subject, user.Id),
                new(JwtClaimTypes.Name, user.UserName ?? string.Empty)
            };
                var token = await identityServerTools.IssueJwtAsync(3600, claims); // 1 hour lifetime

                // --- Refresh Token Generation ---
                var refreshToken = new RefreshToken
                {
                    Token = Guid.NewGuid().ToString("N"), // A secure random string
                    Expires = DateTime.UtcNow.AddDays(30),
                    Created = DateTime.UtcNow
                };
                user.RefreshTokens.Add(refreshToken);
                await dbContext.SaveChangesAsync();

                // --- Set Refresh Token in Secure Cookie ---
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true, // Ensure this is true in production
                    SameSite = SameSiteMode.Strict,
                    Expires = refreshToken.Expires
                };
                httpContext.Response.Cookies.Append("X-Refresh-Token", refreshToken.Token, cookieOptions);

                return Results.Ok(new LoginSuccessResponse { AccessToken = token });
            }

            return Results.Unauthorized();

        }).Produces<LoginSuccessResponse>(StatusCodes.Status200OK)
          .Produces(StatusCodes.Status401Unauthorized)
          .WithName("LoginUser");



        // refresh token endpoint 
        accountGroup.MapPost("/refresh", async (
            HttpContext httpContext,
            ApplicationDbContext dbContext,
            IIdentityServerTools identityServerTools,
            SignInManager<ApplicationUser> signInManager) =>
        {

            //get old refresh token from cookie
            var oldRefreshTokenString = httpContext.Request.Cookies["X-Refresh-Token"];

            if (string.IsNullOrEmpty(oldRefreshTokenString))
            {
                return Results.Unauthorized();
            }

            //find user with this refresh token
            var user = await dbContext.Users
                .Include(u => u.RefreshTokens)
                .SingleOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == oldRefreshTokenString));

            if (user == null)
            {
                return Results.Unauthorized();
            }

            //find the specific token and validate it
            var oldRefreshToken = user.RefreshTokens.Single(t => t.Token == oldRefreshTokenString);

            if (!oldRefreshToken.IsActive)
            {
                return Results.Unauthorized();
            }

            //revoke the old refresh token
            oldRefreshToken.Revoked = DateTime.UtcNow;

            //generate a new access token 
            var claims = new List<Claim>
            {
                new (JwtClaimTypes.Subject, user.Id),
                new (JwtClaimTypes.Name, user.UserName ?? string.Empty)
            };
            var newAccessToken = await identityServerTools.IssueJwtAsync(3600, claims);

            //generate a new refresh token and save it
            var newRefreshToken = new RefreshToken
            {
                Token = Guid.NewGuid().ToString("N"),
                Expires = DateTime.UtcNow.AddDays(30),
                Created = DateTime.UtcNow,
            };

            user.RefreshTokens.Add(newRefreshToken);
            await dbContext.SaveChangesAsync();

            //set a new refresh token cookie
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = newRefreshToken.Expires,
                SameSite = SameSiteMode.Strict,
                Secure = true // Set to true in production
            };

            httpContext.Response.Cookies.Append("X-Refresh-Token", newRefreshToken.Token, cookieOptions);

            return Results.Ok(new LoginSuccessResponse { AccessToken = newAccessToken });

        });

        return app;
    }
}