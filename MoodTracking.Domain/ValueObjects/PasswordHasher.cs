using System;
using System.Security.Cryptography;
using System.Text;
using Konscious.Security.Cryptography;

namespace MoodTracking.Domain.ValueObjects
{
    public static class PasswordHasher
    {
        public static string HashPassword(string password)
        {
            var salt = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
                rng.GetBytes(salt);
            var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
            {
                Salt = salt,
                DegreeOfParallelism = 4,
                MemorySize = 65536,
                Iterations = 2
            };
            var hash = argon2.GetBytes(32);
            return $"{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
        }

        public static bool VerifyPassword(string password, string hashWithSalt)
        {
            var parts = hashWithSalt.Split('.');
            if (parts.Length != 2) return false;
            var salt = Convert.FromBase64String(parts[0]);
            var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
            {
                Salt = salt,
                DegreeOfParallelism = 4,
                MemorySize = 65536,
                Iterations = 2
            };
            var hash = argon2.GetBytes(32);
            return Convert.ToBase64String(hash) == parts[1];
        }
    }
}
