using System.Data;
using Microsoft.Data.SqlClient;
using db.Configuration;
using db.Security;

namespace db.Data
{
    /// <summary>Result of a successful login.</summary>
    public sealed record AuthenticatedUser(string UserName, byte[] Photo, bool IsAdmin);

    public interface IAuthService
    {
        /// <summary>
        /// Looks the user up by name with a parameterized query and verifies the password.
        /// Returns null when the user does not exist or the password is wrong - it never throws for those cases.
        /// </summary>
        AuthenticatedUser? Authenticate(string userName, string password);
    }

    public sealed class AuthService : IAuthService
    {
        private const string AdminUserName = "admin";

        private readonly string connection;

        public AuthService() : this(AppSettings.ConnectionString)
        {
        }

        public AuthService(string connectionString)
        {
            connection = string.IsNullOrWhiteSpace(connectionString)
                ? AppSettings.ConnectionString
                : connectionString;
        }

        public AuthenticatedUser? Authenticate(string userName, string password)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                return null;
            }

            string storedUserName = userName;
            string? storedPassword = null;
            byte[] photo = Array.Empty<byte>();
            bool found = false;

            using (SqlConnection sqlConnection = new SqlConnection(connection))
            {
                sqlConnection.Open();

                const string query = "SELECT username, Password, image FROM Stu1 WHERE username = @username";

                using (SqlCommand command = new SqlCommand(query, sqlConnection))
                {
                    command.Parameters.Add("@username", SqlDbType.NVarChar, 200).Value = userName;

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            found = true;
                            storedUserName = reader["username"] as string ?? userName;
                            storedPassword = reader["Password"] as string;
                            photo = ReadPhoto(reader);
                        }
                    }
                }
            }

            if (!found || !PasswordHasher.Verify(password, storedPassword))
            {
                return null;
            }

            if (PasswordHasher.NeedsUpgrade(storedPassword))
            {
                UpgradeStoredPassword(storedUserName, password);
            }

            return new AuthenticatedUser(
                storedUserName,
                photo,
                string.Equals(storedUserName, AdminUserName, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>Returns an empty array when the image column is absent or NULL.</summary>
        private static byte[] ReadPhoto(SqlDataReader reader)
        {
            int ordinal;
            try
            {
                ordinal = reader.GetOrdinal("image");
            }
            catch (IndexOutOfRangeException)
            {
                return Array.Empty<byte>();
            }

            if (reader.IsDBNull(ordinal))
            {
                return Array.Empty<byte>();
            }

            return reader.GetValue(ordinal) as byte[] ?? Array.Empty<byte>();
        }

        /// <summary>
        /// Best-effort re-hash of a legacy plaintext password after a successful login.
        /// A failure here must not fail the login, so database errors are ignored.
        /// </summary>
        private void UpgradeStoredPassword(string userName, string password)
        {
            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(connection))
                {
                    sqlConnection.Open();

                    const string query = "UPDATE Stu1 SET Password = @password WHERE username = @username";

                    using (SqlCommand command = new SqlCommand(query, sqlConnection))
                    {
                        command.Parameters.Add("@password", SqlDbType.NVarChar, 400).Value = PasswordHasher.Hash(password);
                        command.Parameters.Add("@username", SqlDbType.NVarChar, 200).Value = userName;
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException)
            {
                // The user is already authenticated; the row stays plaintext and will be retried next login.
            }
        }
    }
}
