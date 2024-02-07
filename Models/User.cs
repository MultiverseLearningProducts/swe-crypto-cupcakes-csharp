namespace SweCryptoCupcakesCsharp.Models;

public class User
{
    public long Id { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
}