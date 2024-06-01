namespace NutriBest.Server.Features.Orders.Extensions
{
    using System;
    using System.Security.Cryptography;

    public class TokenGenerator
    {
        public static string GenerateToken()
        {
            using (var cryptoProvider = new RNGCryptoServiceProvider())
            {
                byte[] tokenBytes = new byte[32];
                cryptoProvider.GetBytes(tokenBytes);
                return Convert.ToBase64String(tokenBytes);
            }
        }
    }
}
