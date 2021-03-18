using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Image = System.Drawing.Image;

namespace govgameMapCreator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static Bitmap mapColourGradientBar;
        static Bitmap currentMap;

        public MainWindow()
        {
            InitializeComponent();

            LayerListItem firstLayerListItem = new LayerListItem();
            firstLayerListItem.Width = 342;

            LayersListBox.Items.Add(firstLayerListItem);
        }

        private void AddLayerButton_Click(object sender, RoutedEventArgs e)
        {
            LayerListItem layerListItem = new LayerListItem();
            layerListItem.Width = 342;

            LayersListBox.Items.Add(layerListItem);
        }

        private void GenerateButton_Click(object sender, RoutedEventArgs e)
        {
            mapColourGradientBar = (Bitmap)Image.FromFile("map colour gradient bar.png");

            List<int> seeds = new List<int>();
            foreach (LayerListItem layerListItem in this.LayersListBox.Items)
            {
                seeds.Add(layerListItem.Seed);
            }
            List<float> scales = new List<float>();
            foreach (LayerListItem layerListItem in this.LayersListBox.Items)
            {
                scales.Add(layerListItem.Scale);
            }
            List<int> weights = new List<int>();
            foreach (LayerListItem layerListItem in this.LayersListBox.Items)
            {
                weights.Add(layerListItem.Weight);
            }
            int width = 2000;
            int height = 2000;

            List<int[,]> layers = new List<int[,]>();

            for (int i = 0; i < this.LayersListBox.Items.Count; i++)
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
            MapImage.Source = BitmapToImageSource(finalMapImage);
            currentMap = finalMapImage;
        }

        private BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string filename = string.Empty;

            foreach (LayerListItem layerListItem in this.LayersListBox.Items)
            {
                filename += $"{layerListItem.Seed},{layerListItem.Scale},{layerListItem.Weight};";
            }

            currentMap.Save($"{filename}.png", System.Drawing.Imaging.ImageFormat.Png);
        }
    }
}
