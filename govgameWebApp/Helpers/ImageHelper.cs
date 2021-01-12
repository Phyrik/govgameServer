using System.Drawing;

namespace govgameWebApp.Helpers
{
    public class ImageHelper
    {
        public static Bitmap CropBitmap(Bitmap bitmap, Rectangle cropRectangle)
        {
            return bitmap.Clone(cropRectangle, bitmap.PixelFormat);
        }
    }
}
