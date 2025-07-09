using System.Security;
using MoodTracking.Domain.ValueObjects;

namespace MoodTracking.Domain.ValueObjects
{
    public class SecurePassword
    {
        private readonly SecureString _secureString;

        public string Hash { get; private set; }

        public SecurePassword(string plainPassword)
        {
            _secureString = new SecureString();
            if (!string.IsNullOrEmpty(plainPassword))
            {
                foreach (var c in plainPassword)
                    _secureString.AppendChar(c);
                _secureString.MakeReadOnly();
                Hash = PasswordHasher.HashPassword(plainPassword);
            }
            else
            {
                Hash = string.Empty;
            }
        }

        public SecureString GetSecureString() => _secureString;
    }
}
