namespace db.Configuration
{
    /// <summary>
    /// Central, read-only access to application configuration.
    /// Every value falls back in the order: App.config -> environment variable -> hard-coded default.
    /// Secrets (the SMTP password) are read from the environment only and are never stored in App.config.
    /// </summary>
    public static class AppSettings
    {
        static AppSettings()
        {
            // Makes |DataDirectory| in the App.config connection string resolve next to the executable.
            if (AppDomain.CurrentDomain.GetData("DataDirectory") is not string dataDirectory
                || string.IsNullOrEmpty(dataDirectory))
            {
                AppDomain.CurrentDomain.SetData("DataDirectory", AppDomain.CurrentDomain.BaseDirectory);
            }
        }

        /// <summary>Effective database connection string.</summary>
        public static string ConnectionString
        {
            get
            {
                string? fromConfig = null;
                try
                {
                    fromConfig = global::System.Configuration.ConfigurationManager
                        .ConnectionStrings["Default"]?.ConnectionString;
                }
                catch (global::System.Configuration.ConfigurationErrorsException)
                {
                    // Missing or malformed config file: fall through to the environment / default.
                }

                if (!string.IsNullOrWhiteSpace(fromConfig))
                {
                    return fromConfig;
                }

                string? fromEnvironment = Environment.GetEnvironmentVariable("DB_CONNECTION");
                if (!string.IsNullOrWhiteSpace(fromEnvironment))
                {
                    return fromEnvironment;
                }

                string path = AppDomain.CurrentDomain.BaseDirectory;
                return $@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={path}Stu2.mdf;Integrated Security=True;";
            }
        }

        public static string SmtpHost => Read("Smtp.Host", "SMTP_HOST", "smtp.gmail.com");

        public static int SmtpPort =>
            int.TryParse(Read("Smtp.Port", "SMTP_PORT", ""), out int port) && port > 0 ? port : 587;

        public static bool SmtpUseStartTls =>
            !bool.TryParse(Read("Smtp.UseStartTls", "SMTP_STARTTLS", ""), out bool useStartTls) || useStartTls;

        public static string SmtpUserName => Read("Smtp.UserName", "SMTP_USERNAME", "");

        /// <summary>Read from the SMTP_PASSWORD environment variable only - never from App.config.</summary>
        public static string SmtpPassword => Environment.GetEnvironmentVariable("SMTP_PASSWORD") ?? "";

        public static string SmtpFrom => Read("Smtp.From", "SMTP_FROM", "");

        /// <summary>True when enough SMTP settings are present to actually send mail.</summary>
        public static bool IsSmtpConfigured =>
            !string.IsNullOrWhiteSpace(SmtpHost)
            && !string.IsNullOrWhiteSpace(SmtpUserName)
            && !string.IsNullOrWhiteSpace(SmtpPassword);

        private static string Read(string appSettingKey, string environmentVariable, string fallback)
        {
            string? value = null;
            try
            {
                value = global::System.Configuration.ConfigurationManager.AppSettings[appSettingKey];
            }
            catch (global::System.Configuration.ConfigurationErrorsException)
            {
                // Ignore and fall back to the environment.
            }

            if (!string.IsNullOrWhiteSpace(value))
            {
                return value;
            }

            value = Environment.GetEnvironmentVariable(environmentVariable);
            return string.IsNullOrWhiteSpace(value) ? fallback : value;
        }
    }
}
