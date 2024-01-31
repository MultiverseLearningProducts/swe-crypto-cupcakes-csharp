namespace SweCryptoCupcakesCsharp.Models;

public class Cupcake
{
    public long Id { get; set; }
    public required string Flavor { get; set; }
    public required string Instructions { get; set; }
}