using System.Drawing;
using System.Linq;

namespace govgameSharedClasses.Helpers
{
    public class MapHelper
    {
        public static int GetLocationHeight(int globalX, int globalY)
        {
            int[] seeds = new int[] { 323, 759, 573892, 34221 };
            float[] scales = new float[] { 0.001f, 0.01f, 0.01f, 0.01f };
            int[] weights = new int[] { 10, 1, 1, 1 };

            int[] tops = new int[4];
            int[] bottoms = new int[4];

            for (int i = 0; i < 4; i++)
            {
                SimplexNoise.Noise.Seed = seeds[i];

                int simplexValue = (int)SimplexNoise.Noise.CalcPixel2D(globalX, globalY, scales[i]);

                tops[i] = simplexValue * weights[i];
                bottoms[i] = weights[i];
            }
            SimplexNoise.Noise.Seed = 1;

            return tops.Sum() / bottoms.Sum();
        }

        public enum MapType
        {
            Biome
        }

        static string GetNameOfMapFileFromMapType(MapType mapType)
        {
            switch (mapType)
            {
                case MapType.Biome:
                    return "biome map.png";
                default:
                    return null;
            }
        }

        public static Color GetLocationColour(int globalX, int globalY, MapType mapType)
        {
            Bitmap map = (Bitmap)Image.FromFile($@"\Images\Maps\{GetNameOfMapFileFromMapType(mapType)}");

            return map.GetPixel(globalX, globalY);
        }
    }
}
