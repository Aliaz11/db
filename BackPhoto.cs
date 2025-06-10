using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace db
{
    public static class BackPhoto
    {
        
        public static void BackSet(Form form)
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
