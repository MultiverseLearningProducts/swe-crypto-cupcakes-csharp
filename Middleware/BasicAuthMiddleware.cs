using System.Text;
using SweCryptoCupcakesCsharp.Models;

public class BasicAuthMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IdentityService _identityService;

    public BasicAuthMiddleware(RequestDelegate next, IdentityService identityService)
    {
        _next = next;
        _identityService = identityService;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        string? authHeader = context.Request.Headers["Authorization"];
        if (authHeader != null && authHeader.StartsWith("Basic"))
        {
            // Parse username and password from authorization header
            var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
            var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
            var separator = decodedUsernamePassword.IndexOf(':');
            var username = decodedUsernamePassword.Substring(0, separator);
            var password = decodedUsernamePassword.Substring(separator + 1);
            
            User? user;
            
            if (context.Request.Method == "POST")
            {
                // If a CreateUser request, use identity service to create a user
                user = _identityService.CreateUser(username, password);
            }
            else 
            {
                // Use identity service to authenticate the user
                user = _identityService.AuthenticateUser(username, password);
            }
            // If null, the authentication or creation failed
            if (user == null)
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Unauthorized");
                return;
            }

            // Add the user information to HttpContext Items so it can be used in the controller
            context.Items.Add("User", user);
        }

        // Continue the pipeline
        await _next(context);
    }
}