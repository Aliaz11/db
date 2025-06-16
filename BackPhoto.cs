namespace db
{
    public class BackPhoto : IBackphoto
    {

        public void BackSet(Form form)
        {
            byte[] imageBytes = Resource1.that;
            using (var ms = new System.IO.MemoryStream(imageBytes))
            {
                var image = System.Drawing.Image.FromStream(ms);
                form.BackgroundImage = image;
            }

            form.BackgroundImageLayout = ImageLayout.Stretch;


        }
    }
}
