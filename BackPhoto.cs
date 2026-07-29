namespace db
{
    public class BackPhoto : IBackphoto
    {
        /// <summary>
        /// Keeps the historical background service as a compatibility seam while the modern visual
        /// system uses a calm solid canvas instead of the old full-window photograph.
        /// </summary>
        public void BackSet(Form form)
        {
            if (form == null)
                return;

            ModernTheme.ApplyBackdrop(form);
        }
    }
}
