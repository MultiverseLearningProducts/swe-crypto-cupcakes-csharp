using Auth0.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SweCryptoCupcakesCsharp.Controllers;

[ApiController]
[Route("[controller]")]
public class AccountController : ControllerBase
{
    // home route to instruct users on how to log in
    [HttpGet("/")]
    public ActionResult Home()
    {
        return Ok("Log in at /login or /account/login endpoint to view cupcakes");
    }

    // redirect routes for easy login / logout
    [Route("/login")]
    public IActionResult RedirectToLogin()
    {
        return RedirectToAction("Login", "Account");
    }

    [Route("/logout")]
    public IActionResult RedirectToLogout()
    {
        return RedirectToAction("Logout", "Account");
    }
    
    // implement Auth0 integration to login users
    [HttpGet("login")]
    public async Task Login(string returnUrl = null)
    {
        var authenticationProperties = new LoginAuthenticationPropertiesBuilder()
            // Points to where Auth0 should redirect the user after login
            .WithRedirectUri(returnUrl ?? "/cupcakes")
            .Build();

        await HttpContext.ChallengeAsync(Auth0Constants.AuthenticationScheme, authenticationProperties);
    }

    // implement Auth0 integration to logout users
    [HttpGet("logout")]
    [Authorize]
    public async Task Logout()
    {
        var authenticationProperties = new LogoutAuthenticationPropertiesBuilder()
            // Points to where Auth0 should redirect the user after logout
            .WithRedirectUri(Url.Action("/"))
            .Build();

        await HttpContext.SignOutAsync(Auth0Constants.AuthenticationScheme, authenticationProperties);
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    }
}
