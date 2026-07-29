namespace db
{
    /// <summary>
    /// Single place where one window hands over to the next.
    /// Every form used to do <c>this.Hide()</c> followed by <c>new FormX().Show()</c>, which left the
    /// hidden form alive and undisposed for the rest of the process lifetime.
    /// </summary>
    internal static class Navigation
    {
        private static readonly HashSet<Form> FormsInTransition = new();

        /// <summary>
        /// Prepares <paramref name="next"/> without exposing its designer-sized first frame, then swaps
        /// it with <paramref name="current"/> in one UI turn. This keeps navigation quick and prevents
        /// the resize flash caused by showing two differently sized top-level windows.
        /// </summary>
        internal static void GoTo(Form current, Form next)
        {
            ArgumentNullException.ThrowIfNull(next);

            if (current == null || current.IsDisposed)
            {
                next.Show();
                return;
            }

            // A fast double-click must not open two copies of the destination form.
            if (!FormsInTransition.Add(current))
            {
                next.Dispose();
                return;
            }

            FormWindowState targetState = current.WindowState == FormWindowState.Minimized
                ? FormWindowState.Normal
                : current.WindowState;
            Rectangle targetBounds = targetState == FormWindowState.Normal
                ? current.Bounds
                : current.RestoreBounds;

            if (targetBounds.Width <= 0 || targetBounds.Height <= 0)
            {
                targetBounds = current.Bounds;
            }

            next.StartPosition = FormStartPosition.Manual;
            next.Bounds = targetBounds;
            next.WindowState = targetState;
            next.ShowInTaskbar = current.ShowInTaskbar;
            next.TopMost = current.TopMost;

            // Show invisibly so Load, data binding, theme layout and the first resize all finish
            // before Windows paints the destination. The opacity switch and old-form close happen
            // together on the next UI turn, so there is no slow cross-fade or blank desktop frame.
            next.Opacity = 0D;
            bool completed = false;
            FormClosedEventHandler? closedBeforeShown = null;

            void CompleteTransition()
            {
                if (completed)
                {
                    return;
                }

                completed = true;
                try
                {
                    if (closedBeforeShown != null)
                    {
                        next.FormClosed -= closedBeforeShown;
                    }

                    if (!next.IsDisposed)
                    {
                        next.Opacity = 1D;
                        next.Activate();
                    }

                    if (!current.IsDisposed)
                    {
                        current.Close();
                    }
                }
                finally
                {
                    FormsInTransition.Remove(current);
                }
            }

            EventHandler? shown = null;
            shown = (_, _) =>
            {
                next.Shown -= shown;
                next.BeginInvoke(new Action(CompleteTransition));
            };
            next.Shown += shown;

            closedBeforeShown = (_, _) =>
            {
                next.FormClosed -= closedBeforeShown;
                if (!completed)
                {
                    FormsInTransition.Remove(current);
                }
            };
            next.FormClosed += closedBeforeShown;

            try
            {
                next.Show();
            }
            catch
            {
                next.Shown -= shown;
                next.FormClosed -= closedBeforeShown;
                FormsInTransition.Remove(current);
                next.Dispose();
                throw;
            }
        }
    }
}
