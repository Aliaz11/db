namespace WinFormsApp3
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();

            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += OnThreadException;

            // Navigation.GoTo closes the form it navigates away from, so the run loop must not be tied
            // to one particular form. LastWindowClosesContext ends the loop when no window is left.
            Application.Run(new LastWindowClosesContext(new Form1()));
        }

        private static void OnThreadException(object sender, ThreadExceptionEventArgs e)
        {
            MessageBox.Show(
                "An unexpected error occurred: " + e.Exception.Message,
                "Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }

    /// <summary>
    /// Keeps the message loop running while at least one window is open and exits once the last one
    /// has been closed. Replaces <c>Application.Run(new Form1())</c>, which shut the application down
    /// as soon as the start form was closed even though other windows were still visible.
    /// </summary>
    internal sealed class LastWindowClosesContext : ApplicationContext
    {
        internal LastWindowClosesContext(Form startForm)
        {
            ArgumentNullException.ThrowIfNull(startForm);

            Application.Idle += OnIdle;
            startForm.Show();
        }

        private void OnIdle(object? sender, EventArgs e)
        {
            if (Application.OpenForms.Count == 0)
            {
                Application.Idle -= OnIdle;
                ExitThread();
            }
        }
    }
}
