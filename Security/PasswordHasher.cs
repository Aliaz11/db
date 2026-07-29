using System.Security.Cryptography;
using System.Text;

namespace db.Security
{
    /// <summary>
    /// PBKDF2 (SHA-256) password hashing with a per-password random salt.
    /// Stored format: <c>PBKDF2$&lt;iterations&gt;$&lt;base64 salt&gt;$&lt;base64 hash&gt;</c>.
    /// Anything that does not parse as that format is treated as a legacy plaintext password
    /// so existing rows keep working until they are re-hashed on the next successful login.
    /// </summary>
    public static class PasswordHasher
    {
        private const string Prefix = "PBKDF2";
        private const int Iterations = 100_000;
        private const int SaltSize = 16;
        private const int KeySize = 32;

        public static string Hash(string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
            byte[] key = Rfc2898DeriveBytes.Pbkdf2(
                password ?? string.Empty, salt, Iterations, HashAlgorithmName.SHA256, KeySize);

            return string.Concat(
                Prefix, "$", Iterations.ToString(), "$",
                Convert.ToBase64String(salt), "$", Convert.ToBase64String(key));
        }

        public static bool Verify(string password, string? stored)
        {
            if (string.IsNullOrEmpty(stored))
            {
                return false;
            }

            if (!TryParse(stored, out int iterations, out byte[] salt, out byte[] expected))
            {
                // Legacy plaintext row: still compare in fixed time.
                return CryptographicOperations.FixedTimeEquals(
                    Encoding.UTF8.GetBytes(password ?? string.Empty),
                    Encoding.UTF8.GetBytes(stored));
            }

            byte[] actual = Rfc2898DeriveBytes.Pbkdf2(
                password ?? string.Empty, salt, iterations, HashAlgorithmName.SHA256, expected.Length);

            return CryptographicOperations.FixedTimeEquals(actual, expected);
        }

        /// <summary>True when the stored value is a legacy plaintext password that should be re-hashed.</summary>
        public static bool NeedsUpgrade(string? stored)
        {
            return !string.IsNullOrEmpty(stored) && !TryParse(stored, out _, out _, out _);
        }

        private static bool TryParse(string stored, out int iterations, out byte[] salt, out byte[] hash)
        {
            iterations = 0;
            salt = Array.Empty<byte>();
            hash = Array.Empty<byte>();

            string[] parts = stored.Split('$');
            if (parts.Length != 4 || !string.Equals(parts[0], Prefix, StringComparison.Ordinal))
            {
                return false;
            }

            if (!int.TryParse(parts[1], out iterations) || iterations <= 0)
            {
                return false;
            }

            try
            {
                salt = Convert.FromBase64String(parts[2]);
                hash = Convert.FromBase64String(parts[3]);
            }
            catch (FormatException)
            {
                return false;
            }

            return salt.Length > 0 && hash.Length > 0;
        }
    }
}
