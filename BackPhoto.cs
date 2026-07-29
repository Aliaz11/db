namespace db
{
    public class BackPhoto : IBackphoto
    {
        /// <summary>
        /// Sets the form background from the embedded resource.
        /// Image.FromStream keeps a reference to the stream for the life of the image, so the bytes
        /// are copied into an owned Bitmap and the stream is then safe to dispose. Any background
        /// image this method previously installed is disposed so repeated calls do not leak GDI handles.
        /// </summary>
        public void BackSet(Form form)
        {
            if (form == null)
                return;

            byte[]? imageBytes = Resource1.that;
            if (imageBytes == null || imageBytes.Length == 0)
                return;

            try
            {
                Image? previous = form.BackgroundImage;

                using (var ms = new System.IO.MemoryStream(imageBytes, writable: false))
                using (var source = Image.FromStream(ms))
                {
                    form.BackgroundImage = new Bitmap(source);
                }

                form.BackgroundImageLayout = ImageLayout.Stretch;
                previous?.Dispose();
            }
            catch (ArgumentException)
            {
                // The embedded resource is not a readable image; leave the background untouched.
            }
        }
    }
}
