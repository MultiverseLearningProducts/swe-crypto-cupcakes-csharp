using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using SweCryptoCupcakesCsharp.Models;

public class JwtMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IdentityService _identityService;

    public JwtMiddleware(RequestDelegate next, IdentityService identityService)
    {
        _next = next;
        _identityService = identityService;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        string? authHeader = context.Request.Headers["Authorization"];
        if (authHeader != null && authHeader.StartsWith("Bearer"))
        {
            // Parse username and password from authorization header
            var token = authHeader.Substring("Bearer ".Length).Trim();
            
            ClaimsPrincipal? user;
            // Use identity service to authenticate the user using the given JWT token
            user = _identityService.CheckToken(token);

            // If null, the token validation was unsuccessful
            if (user == null)
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Unauthorized: user doesn't exist or incorrect password");
                return;
            }

            // Storing the authenticated user as a ClaimsPrincipal in HttpContext so that it can be used by the controller
            context.User = user;
        }

        // Continue the pipeline
        await _next(context);
    }
}