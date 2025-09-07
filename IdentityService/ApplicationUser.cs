using Microsoft.AspNetCore.Identity;

namespace IdentityService.Models;

public class ApplicationUser : IdentityUser
{
    // Add any additional properties you need for your application user

    // Navigation property for the collection of refresh tokens.
    // Each user can have multiple refresh tokens (e.g., one for each device).
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}
