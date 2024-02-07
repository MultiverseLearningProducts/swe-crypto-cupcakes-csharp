using System.Text;
using Microsoft.AspNetCore.Mvc;
using SweCryptoCupcakesCsharp.Models;
using SweCryptoCupcakesCsharp.Utilities;

namespace SweCryptoCupcakesCsharp.Controllers;

[ApiController]
[Route("[controller]s")]
public class UserController : ControllerBase
{
    private readonly IdentityService _identityService;

    public UserController(IdentityService identityService)
    {
        _identityService = identityService;
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

    [HttpGet]
    public ActionResult<User> GetUser()
    {
        // Fetch authenticated user from HttpContext Items
        User? user = HttpContext.Items["User"] as User;

        // If null, the authentication failed
        if(user == null)
        {
            return Unauthorized();
        }
        
        // Don't send back hashed password
        return Ok(new {Id = user.Id, Email = user.Email});
    }



}