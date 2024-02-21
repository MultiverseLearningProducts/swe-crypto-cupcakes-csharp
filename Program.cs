using SweCryptoCupcakesCsharp.Data;
using SweCryptoCupcakesCsharp.Utilities;
using SweCryptoCupcakesCsharp.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Auth0.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);

// register services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// use dependency injection to register EncryptUtility as a Singleton service to be created once and used across the application when needed
builder.Services.AddSingleton<EncryptUtility>();

// Auth0 integration

// Cookie configuration for HTTPS
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

// build application
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

CupcakeInitializer.Initialize(app.Services.GetRequiredService<IWebHostEnvironment>());

app.Run();
