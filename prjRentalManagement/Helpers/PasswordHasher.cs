using System;
using System.Security.Cryptography;
using System.Text;

namespace prjRentalManagement.Helpers
{
    public static class PasswordHasher
    {
        public static string Sha256(string rawData)
        {
            if (rawData == null) return null;
            using (var sha256Hash = SHA256.Create())
            {
                var bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                var builder = new StringBuilder();
                foreach (var b in bytes)
                    builder.Append(b.ToString("x2"));
                return builder.ToString();
            }
        }
    }
}
