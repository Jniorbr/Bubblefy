using System.Security.Cryptography;

namespace Bubblefy.Business;

public static class Senha
{
    public static string GerarSalt() => Convert.ToBase64String(RandomNumberGenerator.GetBytes(16));

    public static string GerarHash(string senha, string salt)
    {
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(senha, Convert.FromBase64String(salt), 100000, HashAlgorithmName.SHA256, 32);
        return Convert.ToBase64String(hash);
    }

    public static bool Verificar(string senha, string salt, string hashSalvo)
    {
        byte[] calculado = Convert.FromBase64String(GerarHash(senha, salt));
        byte[] esperado = Convert.FromBase64String(hashSalvo);
        return CryptographicOperations.FixedTimeEquals(calculado, esperado);
    }
}