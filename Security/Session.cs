using db.Data;

namespace db.Security
{
    /// <summary>
    /// The currently signed-in user for this process.
    /// <para>
    /// This exists because the administration screens used to be reachable straight from the main
    /// menu: <c>Form1</c>'s "edit" button opened <c>Form3</c> — which lists every user and can edit
    /// or delete any of them — without asking for credentials. Hardening the login alone would not
    /// have closed that hole, so the admin screens now verify a session instead of trusting the
    /// navigation path that reached them.
    /// </para>
    /// <para>
    /// This is process-wide mutable state, which is not something to copy elsewhere. It is the
    /// smallest change that makes the guard reliable in a WinForms app whose forms construct one
    /// another directly; a proper fix threads the authenticated user through the form constructors.
    /// </para>
    /// </summary>
    internal static class Session
    {
        /// <summary>The signed-in user, or null when nobody is signed in.</summary>
        internal static AuthenticatedUser? Current { get; private set; }

        /// <summary>True only when someone is signed in and that account is an administrator.</summary>
        internal static bool IsAdmin => Current is { IsAdmin: true };

        internal static void SignIn(AuthenticatedUser user)
        {
            ArgumentNullException.ThrowIfNull(user);
            Current = user;
        }

        internal static void SignOut() => Current = null;

        /// <summary>
        /// Shows a "please sign in" message when the current session is not an administrator.
        /// Returns true when the caller should stop what it was doing and send the user to the login form.
        /// </summary>
        internal static bool DenyIfNotAdmin()
        {
            if (IsAdmin)
            {
                return false;
            }

            MessageBox.Show(
                "Please sign in as an administrator to open this screen.",
                "Sign in required",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);

            return true;
        }
    }
}
