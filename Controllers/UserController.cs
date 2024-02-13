using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SweCryptoCupcakesCsharp.Models;
using SweCryptoCupcakesCsharp.Utilities;

namespace SweCryptoCupcakesCsharp.Controllers;

[ApiController]
[Route("[controller]s")]
public class UserController : ControllerBase
{
    private readonly IdentityService _identityService;
    private readonly JwtSettings _jwtSettings;

    public UserController(IdentityService identityService, IOptions<JwtSettings> jwtSettings)
    {
        _identityService = identityService;
        _jwtSettings = jwtSettings.Value;
    }

    [HttpPost]
    public ActionResult<User> CreateUser()
    {
        // Fetch created user from HttpContext Items
        User? user = HttpContext.Items["User"] as User;

        // If null, the creation failed
        if (user == null)
        {
            return BadRequest();
        }

        // Don't send back hashed password
        return Ok(new {Id = user.Id, Email = user.Email});
    }

    [HttpPost("login")]
    public ActionResult Login()
    {
        // Fetch authenticated user from HttpContext Items
        User? user = HttpContext.Items["User"] as User;

        // If null, the authentication failed
        if(user == null)
        {
            return Unauthorized(new {error = "Invalid email or password."});
        }

        var token = _identityService.GenerateToken(user);

        return Ok(new {token, user.Id, user.Email});
    }

    [HttpGet]
    [Authorize]
    public ActionResult<User> GetUser()
    {
        // // Retrieving the authenticated User (ClaimsPrincipal) from HttpContext which is populated by JwtMiddleware
        var user = User;

        // If null, the authentication failed
        if(user == null)
        {
            return Unauthorized(new {error = "Couldn't access user data."});
        }

        // Parsing the user's Id and Email from the user claims
        long Id = long.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0"); 
        string Email = user.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty;
        
        // Don't send back hashed password
        return Ok(new {Id = Id, Email = Email});
    }

}