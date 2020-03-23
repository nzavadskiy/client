using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace client
{
    public partial class PeriodicQueryWindow : Window
    {
        private static readonly Regex _regex = new Regex("[0-9]+");
        public PeriodicQueryWindow()
        {
            InitializeComponent();
        }
        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            if (tbLat.Text != "")
            {
                MainWindow.latencyTime = Convert.ToInt32(tbLat.Text);
                MainWindow.aTimer.Enabled = true;
                Close();
            }
            else
                MessageBox.Show("Enter latency!");
        }
        private static bool IsTextAllowed(string text)
        {
            return _regex.IsMatch(text);
        }
        new void PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }   
    }
}
