namespace NutriBest.Server.Infrastructure.Extensions
{
    using System.Security.Cryptography;

    public class KeyGenerator
    {
        public static string GenerateRandomKey(int length)
        {
            byte[] key = new byte[length];
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(key);
            }
            return Convert.ToBase64String(key);
        }
    }
}
