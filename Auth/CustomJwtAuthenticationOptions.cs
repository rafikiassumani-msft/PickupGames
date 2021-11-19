using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using PickUpGames.Services;
using System.Text;
using System.Text.Encodings.Web;
using System;

namespace PickUpGames.Auth;
public class CustomJwtAuthenticationOptions: AuthenticationSchemeOptions

{

}


public class CustomJwtAuthenticationHandler : AuthenticationHandler<CustomJwtAuthenticationOptions>
{
    private readonly ITokenService _tokenService;

    public CustomJwtAuthenticationHandler(IOptionsMonitor<CustomJwtAuthenticationOptions> options, 
        ILoggerFactory logger, 
        UrlEncoder encoder, 
        ISystemClock clock,
        ITokenService tokenService) : base(options, logger, encoder, clock)
    {
        _tokenService = tokenService;   
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if(!Request.Headers.ContainsKey("Authorization"))
        {
            return Task.FromResult(AuthenticateResult.Fail("No Authorization Header supplied"));
        }

        var authHeader = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ");
        if(authHeader == null || !authHeader.First().Equals("Bearer"))
        {
            return Task.FromResult(AuthenticateResult.Fail("No Bearer token supplied"));
        }

        var accessToken = authHeader.Last();
        var claimsPrincipal = _tokenService.ValidateToken(accessToken);   

        if(claimsPrincipal == null)
        {
            return Task.FromResult(AuthenticateResult.Fail("Invalid access token supplied"));
        }

        var authTicket = new AuthenticationTicket(claimsPrincipal, "CustomJwt");

        return Task.FromResult(AuthenticateResult.Success(authTicket));
    }

    protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        //Need to use the property object from AuthenticateResult.Fail to properly set headers and error messages. 
        Response.Headers.Add("WWW-Authenticate", "Bearer error=Invalid token");
        byte[] result = Encoding.ASCII.GetBytes("Invalid bearer token");
        await Response.Body.WriteAsync(result);

        await base.HandleChallengeAsync(properties);
    }
}