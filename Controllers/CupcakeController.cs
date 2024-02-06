using Microsoft.AspNetCore.Mvc;
using SweCryptoCupcakesCsharp.Models;
using SweCryptoCupcakesCsharp.Utilities;

namespace SweCryptoCupcakesCsharp.Controllers;

[ApiController]
[Route("[controller]s")]
public class CupcakeController : ControllerBase
{
    public static List<Cupcake> cupcakes = new List<Cupcake>();

    public static long uniqueId = cupcakes.Count;

    private readonly ILogger<CupcakeController> _logger;

    private readonly EncryptUtility _encryptUtility;

    public CupcakeController(ILogger<CupcakeController> logger, EncryptUtility encryptUtility)
    {
        _logger = logger;
        // Assign encryptUtility instance to field to be able to use Encrypt and Decrypt functions
        _encryptUtility = encryptUtility;
    }

    [HttpGet]
    public ActionResult<List<Cupcake>> GetCupcakes(string? flavor = null)
    {
        // use Decrypt utility function to decode the cupcake instructions before sending back
        List<Cupcake> decodedCupcakes = cupcakes.ConvertAll<Cupcake>(cupcake => new Cupcake {
            Id = cupcake.Id,
            Flavor = cupcake.Flavor,
            Instructions = _encryptUtility.Decrypt(cupcake.Instructions)
        });

        if (flavor == null)
        {
            return decodedCupcakes;
        }

        List<Cupcake> filteredCupcakes = decodedCupcakes.Where(cupcake => cupcake.Flavor == flavor).ToList();

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

        // use Decrypt utility function to decode the cupcake instructions before sending back
        Cupcake decodedCupcake = new Cupcake {
            Id = foundCupcake.Id,
            Flavor = foundCupcake.Flavor,
            Instructions = _encryptUtility.Decrypt(foundCupcake.Instructions)
        };

        return decodedCupcake;
    }

    [HttpPost]
    public ActionResult<Cupcake> PostCupcake(Cupcake cupcake)
    {
        cupcake.Id = ++uniqueId;

        // use Encrypt utility function to decode the cupcake instructions before sending back
        cupcake.Instructions = _encryptUtility.Encrypt(cupcake.Instructions);

        cupcakes.Add(cupcake);

        return CreatedAtAction("GetCupcake", new {id = cupcake.Id}, cupcake);
    }
}
