using SweCryptoCupcakesCsharp.Data;
using SweCryptoCupcakesCsharp.Utilities;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// use dependency injection to register EncryptUtility as a Singleton service to be created once and used across the application when needed
builder.Services.AddSingleton<EncryptUtility>();
// add IdentityService for basic auth
builder.Services.AddSingleton<IdentityService>();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseMiddleware<BasicAuthMiddleware>();

app.UseAuthorization();

app.MapControllers();

CupcakeInitializer.Initialize(app.Services.GetRequiredService<IWebHostEnvironment>());

app.Run();
