using System;
using Newtonsoft.Json;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using SweCryptoCupcakesCsharp.Models;
using SweCryptoCupcakesCsharp.Controllers;

namespace SweCryptoCupcakesCsharp.Data;

public static class CupcakeInitializer
{
    public static void Initialize(IWebHostEnvironment env)
    {
        var cupcakesJson = File.ReadAllText(Path.Combine(env.ContentRootPath, "Data", "seedData.json"));

        var cupcakesFromJson = JsonConvert.DeserializeObject<List<Cupcake>>(cupcakesJson);

        CupcakeController.cupcakes.AddRange(cupcakesFromJson?.ToList() ?? new List<Cupcake>());
        CupcakeController.uniqueId = CupcakeController.cupcakes.Count();
    }
}