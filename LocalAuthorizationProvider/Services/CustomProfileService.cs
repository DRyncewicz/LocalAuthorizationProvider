using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using IdentityModel;
using LocalAuthorizationProvider.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace LocalAuthorizationProvider.Services;

public class CustomProfileService(UserManager<ApplicationUser> _userManager) : IProfileService
{
    private readonly UserManager<ApplicationUser> _userManager = _userManager;

    public async Task GetProfileDataAsync(ProfileDataRequestContext context)
    {
        var user = await _userManager.GetUserAsync(context.Subject);

        var claims = new List<Claim>
    {
        new Claim(JwtClaimTypes.Name, user.UserName),
        new Claim(JwtClaimTypes.Email, user.Email)
    };

        if (context.RequestedClaimTypes.Contains(JwtClaimTypes.Name))
        {
            claims.Add(new Claim(JwtClaimTypes.Name, user.UserName));
        }

        if (context.RequestedClaimTypes.Contains(JwtClaimTypes.Email))
        {
            claims.Add(new Claim(JwtClaimTypes.Email, user.Email));
        }

        context.IssuedClaims.AddRange(claims);
    }

    public async Task IsActiveAsync(IsActiveContext context)
    {
        var user = await _userManager.GetUserAsync(context.Subject);
        context.IsActive = user != null;
    }
}