# SWE Crypto Cupcakes - C#

**See setup instructions on the `main` branch to install dependencies and start
the ASP.NET Core server.**

This branch removes some of the work we did last time: we no longer have to manually implement `/users` endpoints for signing up, logging in, or logging out. These are now provided by the Auth0 SDK (software development kit) through the Nuget package `Auth0.AspNetCore.Authentication`. We were able to remove the Middleware classes, the IdentityService class, and the JwtSettings model. This refactor should allow us to sign in with Google, and benefit from all the goodness Auth0 provides: user management database, account merging, premade middleware and UI, no-hassle security updates and a team of security experts monitoring the landscape at all times. 

The past two sessions have been quite heavy - lots of manually handling tokens
etc. This session should feel like a bit of a relief because Auth0 handles a lot
of the difficulty for us.

## Coach notes

Set up your Auth0 Domain, ClientId, and ClientSecret as user-secrets using .NET's Secret Manager Tool. As a reference for this demo, this information is stored in `secrets.json`. However, these secrets should usually NOT be committed to the repository, and they should always be stored securely using environment variables or a secret manager. The values below will technically work, but see below for instructions to create an Auth0 account and generate your own application and related configuration values.

Set new secrets for the Domain (issuer base url), ClientId, and ClientSecret. You can see it in JSON format in `secrets.json`.

```bash
dotnet user-secrets set "Auth0:Domain" "dev-qdcrhjic0t4oqyqr.us.auth0.com"
```

```bash
dotnet user-secrets set "Auth0:ClientId" "SJXEXbtBh03DyFh2mnYwuF7TKipdGLjA"
```

```bash
dotnet user-secrets set "Auth0:ClientSecret" "FMSlM6M07xTfVMGgSOQtWjfgDSNVK-uGqAe5fqyJNFtvymJt0nV45l6k4k8qoM8H"
```

View your secrets using:

```bash
dotnet user-secrets list
```

In `Program.cs`, you will notice new configuration using `Configure` and `AddAuth0WebAppAuthentication`: this is for Auth0.

```c#
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.MinimumSameSitePolicy = SameSiteMode.None;
});

builder.Services.AddAuth0WebAppAuthentication(options =>
{
    IConfigurationSection auth0Config = builder.Configuration.GetSection("Auth0");

    options.Domain = auth0Config["Domain"];
    options.ClientId = auth0Config["ClientId"];
    options.ClientSecret = auth0Config["ClientSecret"];
    options.Scope = "openid profile email";
});
```

`AccountController.cs` was created for the new `/account/login` and `/account/logout` endpoints using the Auth0 documentation. You'll also see redirects so `/login` and `/logout` hit those same endpoints:

```c#
[HttpGet("login")]
public async Task Login()
{
    var authenticationProperties = new LoginAuthenticationPropertiesBuilder()
        .WithRedirectUri("/cupcakes")
        .Build();

    await HttpContext.ChallengeAsync(Auth0Constants.AuthenticationScheme, authenticationProperties);
}

[HttpGet("logout")]
[Authorize]
public async Task Logout()
{
    var authenticationProperties = new LogoutAuthenticationPropertiesBuilder()
        .WithRedirectUri(Url.Action("/"))
        .Build();

    await HttpContext.SignOutAsync(Auth0Constants.AuthenticationScheme, authenticationProperties);
    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
}
```

You'll also notice that `UserController.cs` has been updated to return information about the logged-in user using .NET's built-in identity system and claims from Auth0.

```c#
[HttpGet]
    [Authorize]
    public ActionResult<User> GetUser()
    {
        return Ok( new {
            Name = User.Identity.Name,
            EmailAddress = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value,
            ProfileImage = User.Claims.FirstOrDefault(c => c.Type == "picture")?.Value,
            Sub = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value
        });
    }
```

Whlist the app would probably work on your machine, you wouldn't be able to go
to Auth0 and show the new user being added to the user management dashboard,
because the client id and issuer base url (domain) are registered to
david.todd@multiverse.io. It would be a great idea to get hold of your own
values for these by setting up your own Auth0 account and making a SWE Crypto Cupcakes
application - follow the tutorial for an ASP.NET MVC traditional web app to see
how the whole process works.

## Things to see and do

### Auth0 website

Take a look at the website and the documentation. Take a look at how the steps
have been implemented:

- `Auth0.AspNetCore.Authentication` is the SDK in `swe-crypto-cupcakes-csharp.csproj`
- the configuration steps in `Program.cs` (this should be done before other routes are registered)
- `AccountController.cs` implementation of `account/login` and `account/logout` endpoints
- the `[Authorize]` middleware on endpoints in various routes, similar to last week's implementation
- the `User.Claims` and `User.Identity` objects in the GET `/users` endpoint

### Config

Notice that we still need a secret to give to the Auth0 configuration in `Program.cs`. Why is this?
What is it for?

What is the client id?

What is the difference between the base url (e.g. `applicationUrl` defined in `launchSettings.json`) and the issuer base url (domain "defined" in `secrets.json`, but stored in user-secrets)?

### Protected endpoints

Run the app with `dotnet watch run --launch-profile https` and try visiting `/cupcakes`. What happens? How is this related to the configuration being defined in `Program.cs`?

### Create an account

Try making an email/password account. Try making one with a third party
provider.

Notice that signing up also logs you in, which is a much better UX.

Take a look at the user management in Auth0.

## Next steps

Auth0 have SDKs for most languages/frameworks
[here](https://auth0.com/docs/quickstart/webapp#webapp). Auth0 provide solutions
for pure APIs, web apps, SPAs and mobile apps. The pure API solution is
essentially a reimplementation of the JWT flow we have already done, and isn't
the best way to get OIDC working. We strongly recommend following the tutorial
for "traditional web app" - as a bonus, it comes with a "Login with Google"
integration by default (this hasn't been tested on all languages, though - some
additional setup could be needed for this).

The Auth0 docs are really strong, and there are sample projects available.

### Extension

There should be plenty to be getting on with, but you could challenge
apprentices to add "sign in with Github" to their Auth0 solution.

Could they use the access token to read some protected information from their
Google or Github account? (They may need to add scopes!)