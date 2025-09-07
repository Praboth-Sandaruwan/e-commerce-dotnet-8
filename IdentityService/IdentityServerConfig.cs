using Duende.IdentityServer.Models;
using System.Collections.Generic;

public static class IdentityServerConfig
{
    // Defines the API scopes. Scopes are granular permissions that clients can request.
    public static IEnumerable<ApiScope> GetApiScopes()
    {
        return new List<ApiScope>
        {
            // "ecommerce.api" is the scope name. The second parameter is a display name for consent screens.
            new ApiScope("ecommerce.api", "Full access to E-Commerce API")
        };
    }

    public static IEnumerable<Client> GetClients()
    {
        return new List<Client>
        {
            new Client
            {
                ClientId = "web_client", // Unique identifier for the client (e.g., our future web app)
                ClientName = "E-Commerce Web Client",
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                // In a real app, this MUST be stored securely and not hardcoded.
                ClientSecrets = { new Secret("a_very_secure_secret_!@#$".Sha256()) },
                // The list of scopes that this client is allowed to request.
                AllowedScopes = { "ecommerce.api" }
            }
        };
    }
}