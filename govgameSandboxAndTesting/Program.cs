using System.Collections.Generic;
using System.Drawing;

namespace govgameSandboxAndTesting
{
    class Program
    {
        /* details:
         * 
         * map.png - seed = 1, scale = 1, size = 100x100, grey
         * map1.png - seed = 1, scale = 0.5, size = 100x100, grey
         * map2.png - seed = 1, scale = 0.1, size = 100x100, grey
         * map3.png - seed = 1, scale = 0.05, size = 100x100, grey
         * map4.png - seed = 1, scale = 0.01, size = 100x100, grey
         * map5.png - seed = 1, scale = 0.01, size = 500x500, grey
         * map6.png - seed = 1, scale = 0.002, size = 500x500, grey
         * map7.png - seed = 1, scale = 0.002, size = 500x500, map colour gradient
         * map8.png - seed = 1, scale = 0.002, size = 2000x2000, map colour gradient
         * map9.png - seed = 1, scales = 0.002 + 0.01, size = 2000x2000, weights = 2 + 1, map colour gradient
         * map10.png - seed = 1, scales = 0.002 + 0.01 + 0.1, size = 2000x2000, weights = 3 + 2 + 1, map colour gradient
         * map11.png - seed = 1, scales = 0.002 + 0.01 + 0.1, size = 2000x2000, weights = 4 + 2 + 1, map colour gradient
         * map12.png - seeds = 1x3 + 2, scales = 0.002 + 0.01 + 0.1 + 0.0075, size = 2000x2000, weights = 4 + 3 + 2 + 1, map colour gradient
         */

        static Bitmap mapColourGradientBar;

        static void Main(string[] args)
        {
            mapColourGradientBar = (Bitmap)Image.FromFile("map colour gradient bar.png");

            int[] seeds = new int[] { 1, 1, 1, 2 };
            float[] scales = new float[] { 0.002f, 0.01f, 0.1f, 0.0075f };
            int[] weights = new int[] { 4, 3, 2, 1 };
            int width = 2000;
            int height = 2000;

            List<int[,]> layers = new List<int[,]>();

            for (int i = 0; i < 4; i++)
            {
                int[,] layer = new int[width, height];
                SimplexNoise.Noise.Seed = seeds[i];
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        int simplexValue = (int)SimplexNoise.Noise.CalcPixel2D(x, y, scales[i]);
                        layer[x, y] = simplexValue;
                    }
                }
                layers.Add(layer);
            }

            int[,] finalLayer = new int[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    int top = 0;
                    int bottom = 0;
                    for (int i = 0; i < layers.Count; i++)
                    {
                        top += weights[i] * layers[i][x, y];
                        bottom += weights[i];
                    }

                    finalLayer[x, y] = top / bottom;
                }
            }

            Bitmap finalMapImage = new Bitmap(width, height);
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    finalMapImage.SetPixel(x, y, mapColourGradientBar.GetPixel(0, finalLayer[x, y]));
                }
            }
            finalMapImage.Save("map12.png", System.Drawing.Imaging.ImageFormat.Png);
        }
    }
}
