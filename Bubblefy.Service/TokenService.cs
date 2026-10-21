<<<<<<< HEAD
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Bubblefy.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

=======
using Bubblefy.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
>>>>>>> 95f8a9d55bf0db36e594832008fcf298de01dafa
public class TokenService
{
    private readonly IConfiguration _configuration;
    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }
 
    public static object GetSigningKey(ConfigurationManager configuration)
    {
        throw new NotImplementedException();
    }
 
    public string GenerateToken(Usuario usuario)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtSettings = _configuration.GetSection("JWTSettings");
        var chaveSecreta = Encoding.ASCII.GetBytes(jwtSettings["SecretKey"]);
        var credenciais = new SigningCredentials(new SymmetricSecurityKey(chaveSecreta),
        SecurityAlgorithms.HmacSha256Signature);
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, usuario.Nome),
        };
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            SigningCredentials = credenciais,
            Expires = DateTime.UtcNow.AddMinutes(double.Parse(jwtSettings["ExpirationInMinutes"])),
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}