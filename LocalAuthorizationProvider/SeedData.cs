using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Mappers;
using Duende.IdentityServer.Models;
using IdentityModel;
using LocalAuthorizationProvider.Data;
using LocalAuthorizationProvider.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Security.Claims;

namespace LocalAuthorizationProvider
{
    public class SeedData
    {
        public static void EnsureSeedData(WebApplication app)
        {
            using (var scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var context = scope.ServiceProvider.GetService<ApplicationDbContext>();
                context.Database.Migrate();

                var configurationDbContext = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                configurationDbContext.Database.Migrate();

                if (!configurationDbContext.Clients.Any())
                {
                    Log.Debug("Clients being populated");
                    foreach (var client in Config.Clients.ToList())
                    {
                        configurationDbContext.Clients.Add(client.ToEntity());
                    }
                    configurationDbContext.SaveChanges();
                }
                else
                {
                    Log.Debug("Clients already populated");
                }

                if (!configurationDbContext.IdentityResources.Any())
                {
                    Log.Debug("IdentityResources being populated");
                    foreach (var resource in Config.IdentityResources.ToList())
                    {
                        configurationDbContext.IdentityResources.Add(resource.ToEntity());
                    }
                    configurationDbContext.SaveChanges();
                }
                else
                {
                    Log.Debug("IdentityResources already populated");
                }

                if (!configurationDbContext.ApiScopes.Any())
                {
                    Log.Debug("ApiScopes being populated");
                    foreach (var resource in Config.ApiScopes.ToList())
                    {
                        configurationDbContext.ApiScopes.Add(resource.ToEntity());
                    }
                    configurationDbContext.SaveChanges();
                }
                else
                {
                    Log.Debug("ApiScopes already populated");
                }

                if (!configurationDbContext.IdentityProviders.Any())
                {
                    Log.Debug("OIDC IdentityProviders being populated");
                    configurationDbContext.IdentityProviders.Add(new OidcProvider
                    {
                        Scheme = "demoidsrv",
                        DisplayName = "IdentityServer",
                        Authority = "https://demo.duendesoftware.com",
                        ClientId = "login",
                    }.ToEntity());
                    configurationDbContext.SaveChanges();
                }
                else
                {
                    Log.Debug("OIDC IdentityProviders already populated");
                }

                var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var alice = userMgr.FindByNameAsync("alice").Result;
                if (alice == null)
                {
                    alice = new ApplicationUser
                    {
                        UserName = "alice",
                        Email = "AliceSmith@email.com",
                        EmailConfirmed = true,
                    };
                    var result = userMgr.CreateAsync(alice, "Pass123$").Result;
                    if (!result.Succeeded)
                    {
                        throw new Exception(result.Errors.First().Description);
                    }

                    result = userMgr.AddClaimsAsync(alice, new Claim[]{
                                new Claim(JwtClaimTypes.Name, "Alice Smith"),
                                new Claim(JwtClaimTypes.GivenName, "Alice"),
                                new Claim(JwtClaimTypes.FamilyName, "Smith"),
                                new Claim(JwtClaimTypes.WebSite, "http://alice.com"),
                            }).Result;
                    if (!result.Succeeded)
                    {
                        throw new Exception(result.Errors.First().Description);
                    }
                    Log.Debug("alice created");
                }
                else
                {
                    Log.Debug("alice already exists");
                }

                var bob = userMgr.FindByNameAsync("bob").Result;
                if (bob == null)
                {
                    bob = new ApplicationUser
                    {
                        UserName = "bob",
                        Email = "BobSmith@email.com",
                        EmailConfirmed = true
                    };
                    var result = userMgr.CreateAsync(bob, "Pass123$").Result;
                    if (!result.Succeeded)
                    {
                        throw new Exception(result.Errors.First().Description);
                    }

                    result = userMgr.AddClaimsAsync(bob, new Claim[]{
                                new Claim(JwtClaimTypes.Name, "Bob Smith"),
                                new Claim(JwtClaimTypes.GivenName, "Bob"),
                                new Claim(JwtClaimTypes.FamilyName, "Smith"),
                                new Claim(JwtClaimTypes.WebSite, "http://bob.com"),
                                new Claim("location", "somewhere")
                            }).Result;
                    if (!result.Succeeded)
                    {
                        throw new Exception(result.Errors.First().Description);
                    }
                    Log.Debug("bob created");
                }
                else
                {
                    Log.Debug("bob already exists");
                }
            }
        }
    }
}
