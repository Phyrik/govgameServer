using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace govgameMapCreator
{
    /// <summary>
    /// Interaction logic for LayerListItem.xaml
    /// </summary>
    public partial class LayerListItem : UserControl
    {
        public int Seed { get; set; }
        public float Scale { get; set; }
        public int Weight { get; set; }

        public LayerListItem()
        {
            InitializeComponent();
        }

        private void SeedTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                this.Seed = int.Parse(SeedTextBox.Text);
            }
            catch { }
        }

        private void ScaleTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                this.Scale = float.Parse(ScaleTextBox.Text);
            }
            catch { }
        }

        private void WeightTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                this.Weight = int.Parse(WeightTextBox.Text);
            }
            catch { }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ((ListBox)this.Parent).Items.Remove(this);
        }
    }
}
