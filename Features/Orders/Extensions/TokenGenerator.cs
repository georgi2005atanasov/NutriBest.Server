namespace NutriBest.Server.Features.Orders.Extensions
{
    using System;
    using System.Security.Cryptography;

    public class TokenGenerator
    {
        public static string GenerateToken()
        {
            byte[] tokenBytes = new byte[32];
            RandomNumberGenerator.Fill(tokenBytes);
            return Convert.ToBase64String(tokenBytes);
        }
    }
}
