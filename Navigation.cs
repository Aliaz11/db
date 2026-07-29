namespace db
{
    /// <summary>
    /// Single place where one window hands over to the next.
    /// Every form used to do <c>this.Hide()</c> followed by <c>new FormX().Show()</c>, which left the
    /// hidden form alive and undisposed for the rest of the process lifetime.
    /// </summary>
    internal static class Navigation
    {
        /// <summary>
        /// Shows <paramref name="next"/> at the position and size of <paramref name="current"/> and then
        /// closes (and therefore disposes) <paramref name="current"/>.
        /// The next form is shown before the current one is closed so the application never has zero
        /// open windows in between.
        /// </summary>
        internal static void GoTo(Form current, Form next)
        {
            ArgumentNullException.ThrowIfNull(next);

            if (current == null)
            {
                next.Show();
                return;
            }

            next.StartPosition = FormStartPosition.Manual;
            next.Location = current.Location;
            next.Size = current.Size;

            next.Show();
            current.Close();
        }
    }
}
