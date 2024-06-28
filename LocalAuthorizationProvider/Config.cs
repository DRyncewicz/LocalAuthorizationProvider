using Duende.IdentityServer.Models;
using IdentityModel;

namespace LocalAuthorizationProvider;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        new IdentityResource[]
        {
        new IdentityResources.OpenId(),
        new IdentityResources.Profile(),
        new IdentityResource(
            name: "user",
             userClaims: new[] {JwtClaimTypes.Email, JwtClaimTypes.Role}
             ),
        };

    public static IEnumerable<ApiScope> ApiScopes =>
        new ApiScope[]
        {
        new ApiScope("scope1"),
        new ApiScope("scope2"),
        new ApiScope("api1")
        };

    public static IEnumerable<Client> Clients =>
        new Client[]
        {
        new Client
        {
            ClientId = "client",
            ClientName = "Client for Postman user",
            AllowedGrantTypes = GrantTypes.ResourceOwnerPasswordAndClientCredentials,
            ClientSecrets = {new  Secret("secret".Sha256())},
            AllowedScopes = {"api1",  "user"},
            AlwaysSendClientClaims = true,
            AlwaysIncludeUserClaimsInIdToken = true,
            AllowAccessTokensViaBrowser = true
        },
        new Client
        {
            ClientId = "swagger",
            ClientName ="Client for Swagger user",
            AllowedGrantTypes = GrantTypes.CodeAndClientCredentials,
            ClientSecrets = {new Secret("secret".Sha256())},
            AllowedScopes = {"api1", "user", "openid"},
            AlwaysSendClientClaims = true,
            AlwaysIncludeUserClaimsInIdToken = true,
            AllowAccessTokensViaBrowser = true,
            RedirectUris = { "https://localhost:7008/swagger/oauth2-redirect.html" },
            AllowedCorsOrigins = { "https://localhost:7008" }
        },
        new Client
        {
        ClientId = "botickr-client",
        ClientName = "Botickr React Client",
        AllowedGrantTypes = GrantTypes.Code,
        RequireClientSecret = false,
        RedirectUris = { "https://localhost:3000/authentication/login-callback" },
        PostLogoutRedirectUris = { "https://localhost:3000/authentication/logout-callback" },
        AllowedCorsOrigins = { "https://localhost:3000" },
        AllowedScopes = {"user", "openid", "profile", "api1"},
        RequirePkce = true,
        AllowAccessTokensViaBrowser = true
        }

        };
}