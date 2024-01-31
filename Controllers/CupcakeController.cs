using Microsoft.AspNetCore.Mvc;
using SweCryptoCupcakesCsharp.Models;

namespace SweCryptoCupcakesCsharp.Controllers;


[ApiController]
[Route("[controller]s")]
public class CupcakeController : ControllerBase
{
    public static List<Cupcake> cupcakes = new List<Cupcake>();

    public static long uniqueId = cupcakes.Count;

    private readonly ILogger<CupcakeController> _logger;

    public CupcakeController(ILogger<CupcakeController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public ActionResult<List<Cupcake>> GetCupcakes(string? flavor = null)
    {
        if (flavor == null)
        {
            return cupcakes;
        }

        List<Cupcake> filteredCupcakes = cupcakes.Where(cupcake => cupcake.Flavor == flavor).ToList();

        return filteredCupcakes;
    }

    [HttpGet("{id}")]
    public ActionResult<Cupcake> GetCupcake(long id)
    {
        Cupcake? foundCupcake = cupcakes.Find(cupcake => cupcake.Id == id);

        if (foundCupcake == null)
        {
            return NotFound("Cupcake not found.");
        }

        return foundCupcake;
    }

    [HttpPost]
    public ActionResult<Cupcake> PostCupcake(Cupcake cupcake)
    {
        cupcake.Id = ++uniqueId;
        cupcakes.Add(cupcake);

        return CreatedAtAction("GetCupcake", new {id = cupcake.Id}, cupcake);
    }
}
