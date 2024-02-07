using System.Collections;
using System.Security.Cryptography;
using SweCryptoCupcakesCsharp.Models;

public interface IIdentityService
{
    User CreateUser(string email, string password);
    User? AuthenticateUser(string email, string password);
}

public class IdentityService : IIdentityService
{
    private List<User> _users = new List<User>();
    private long uniqueId = 0;

    public User CreateUser(string email, string password)
    {
        var hashedPassword = "";
        using (var deriveBytes = new Rfc2898DeriveBytes(password, 20, 10000, HashAlgorithmName.SHA256))
        {
            byte[] salt = deriveBytes.Salt;
            byte[] buffer = deriveBytes.GetBytes(20);
            hashedPassword = Convert.ToBase64String(salt) + Convert.ToBase64String(buffer);
        }
        var user = new User {Id = ++uniqueId, Email = email, Password = hashedPassword};
        _users.Add(user);

        return user;
    }

    public User? AuthenticateUser(string email, string password)
    {
        var user = _users.Find(user => user.Email == email);
        
        if (user != null)
        {
            var saltString = user.Password.Substring(0,28);
            var savedPasswordHashString = user.Password.Substring(28);

            using (var deriveBytes = new Rfc2898DeriveBytes(password, Convert.FromBase64String(saltString),10000, HashAlgorithmName.SHA256))
            {
                byte[] testPasswordHash = deriveBytes.GetBytes(20);

                if (Convert.ToBase64String(testPasswordHash) == savedPasswordHashString)
                {
                    return user;
                }
            }
        }
        return null;
    }
}