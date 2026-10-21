using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Bubblefy.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Bubblefy.Service;

public class TokenService(IConfiguration configuration) : ITokenService
{
    public static SymmetricSecurityKey GetSigningKey(IConfiguration configuration)
    {
        string secretKey = configuration["JWTSettings:SecretKey"]
            ?? throw new InvalidOperationException("JWTSettings:SecretKey nao foi configurada.");
        byte[] keyBytes = Encoding.UTF8.GetBytes(secretKey);
        if (keyBytes.Length < 32)
            throw new InvalidOperationException("JWTSettings:SecretKey deve ter ao menos 32 bytes.");

        return new SymmetricSecurityKey(keyBytes);
    }

    public string GenerateToken(Usuario usuario)
    {
        string expirationSetting = configuration["JWTSettings:ExpirationInMinutes"]
            ?? throw new InvalidOperationException("JWTSettings:ExpirationInMinutes nao foi configurada.");
        if (!double.TryParse(expirationSetting, System.Globalization.NumberStyles.Number,
                System.Globalization.CultureInfo.InvariantCulture, out double expirationInMinutes)
            || expirationInMinutes <= 0)
        {
            throw new InvalidOperationException("JWTSettings:ExpirationInMinutes deve ser um numero positivo.");
        }

        var tokenHandler = new JwtSecurityTokenHandler();
        var credentials = new SigningCredentials(GetSigningKey(configuration), SecurityAlgorithms.HmacSha256);
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, usuario.Id.ToString(System.Globalization.CultureInfo.InvariantCulture)),
            new(ClaimTypes.Name, usuario.Nome)
        };
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            SigningCredentials = credentials,
            Expires = DateTime.UtcNow.AddMinutes(expirationInMinutes)
        };

        return tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));
    }
}