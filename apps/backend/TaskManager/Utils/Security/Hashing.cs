using System;
using System.Security.Cryptography;

namespace TaskManager.Utils.Security
{
    public abstract class Hashing
    {
        public static (string hash, string salt) HashPassword(string password)
        {
            var salt = new byte[16];
            
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(salt);

            var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, 100000, HashAlgorithmName.SHA256, 32);

            return (Convert.ToBase64String(hash), Convert.ToBase64String(salt));
        }

        public static bool ValidatePassword(string senha, string hashBase64, string saltBase64)
        {
            var salt = Convert.FromBase64String(saltBase64);
            var hashSalvo = Convert.FromBase64String(hashBase64);

            var newHash = Rfc2898DeriveBytes.Pbkdf2(senha, salt, 100000, HashAlgorithmName.SHA256, 32);

            return CryptographicOperations.FixedTimeEquals(newHash, hashSalvo);
        }
    }
}