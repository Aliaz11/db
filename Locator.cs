using db.Configuration;

namespace db
{
    public static class Locator
    {
        /// <summary>Effective database connection string; delegates to <see cref="AppSettings.ConnectionString"/>.</summary>
        public static string GetConnectionString() => AppSettings.ConnectionString;
    }
}
