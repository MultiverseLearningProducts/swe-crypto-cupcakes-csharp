using System.Collections;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SweCryptoCupcakesCsharp.Models;

public interface IIdentityService
{
    User CreateUser(string email, string password);
    User? AuthenticateUser(string email, string password);
    string GenerateToken(User user);
    User? GetUserById(long id);
}

public class IdentityService : IIdentityService
{
    private List<User> _users = new List<User>();
    private long uniqueId = 0;
    private readonly JwtSettings? _jwtSettings;

    public IdentityService(IOptions<JwtSettings> jwtSettings)
    {
        _jwtSettings = jwtSettings.Value;
    }

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

    public string GenerateToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        if(_jwtSettings is null || _jwtSettings.Secret is null)
        {
            throw new ApplicationException("JwtSettings.Secret is null. Ensure it is set in configuration.");
        }
        var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
            }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public User? GetUserById(long id)
    {
        if (_users == null) return null;
        return _users.Find(user => user.Id == id);
    }

    public ClaimsPrincipal? CheckToken(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        if(_jwtSettings is null || _jwtSettings.Secret is null)
        {
            throw new ApplicationException("JwtSettings.Secret is null. Ensure it is set in configuration.");
        }
        var validations = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret)),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero 
        };
        try
        {
            // Validate the token
            var claims = handler.ValidateToken(token, validations, out var tokenSecure);
            // If successful, return a ClaimsPrincipal containing the claims from the token
            return claims;
        }
        catch (SecurityTokenValidationException)
        {
            // Token validation failed
            return null;
        }
        catch (ArgumentException)
        {
            // The token was not well-formed
            return null;
        }
        }

}