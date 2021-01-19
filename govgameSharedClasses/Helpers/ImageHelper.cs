using System.Drawing;

namespace govgameSharedClasses.Helpers
{
    public class ImageHelper
    {
        public static Bitmap CropBitmap(Bitmap bitmap, Rectangle cropRectangle)
        {
            return bitmap.Clone(cropRectangle, bitmap.PixelFormat);
        }
    }
}
