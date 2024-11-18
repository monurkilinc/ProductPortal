using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ProductPortal.Core.Utilities.Security
{
    public static class HashingHelper
    {
        public static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

                // Debug için hash ve salt değerlerini yazdır
                Console.WriteLine($"Creating hash for password: {password}");
                Console.WriteLine($"Generated Hash: {Convert.ToBase64String(passwordHash)}");
                Console.WriteLine($"Generated Salt: {Convert.ToBase64String(passwordSalt)}");
            }
        }

        public static bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

                // Debug için hash değerlerini karşılaştır
                Console.WriteLine($"Verifying password: {password}");
                Console.WriteLine($"Stored Hash: {Convert.ToBase64String(passwordHash)}");
                Console.WriteLine($"Computed Hash: {Convert.ToBase64String(computedHash)}");

                return computedHash.SequenceEqual(passwordHash);
            }
        }
    }
}
