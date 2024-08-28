using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using IdentityModel;
using LocalAuthorizationProvider.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace LocalAuthorizationProvider.Services;

public class CustomProfileService(UserManager<ApplicationUser> _userManager, ISessionManagementService sessionManagementService) : IProfileService
{
    private readonly UserManager<ApplicationUser> _userManager = _userManager;
    private readonly ISessionManagementService _sessionManagementService = sessionManagementService;

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
        var userClaims = await _userManager.GetClaimsAsync(user);
        claims.AddRange(userClaims);

        context.IssuedClaims.AddRange(claims);
    }

    public async Task IsActiveAsync(IsActiveContext context)
    {
        var sessions = await _sessionManagementService.QuerySessionsAsync(new Duende.IdentityServer.Stores.SessionQuery()
        {
            SubjectId = context.Subject.Identity.GetSubjectId()
        });
        if (sessions.Results.Any())
        {
            context.IsActive = true;
        }
        else
        {
            context.IsActive = false;
        }
    }
}