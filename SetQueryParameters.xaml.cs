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
    public partial class SetQueryParameters : Window
    {
        private static readonly Regex _regexInt = new Regex("[0-9]+");
        private static readonly Regex _regexDouble = new Regex(@"^[0-9]*(?:\.[0-9]*)?$");
        public SetQueryParameters()
        {
            InitializeComponent();
        }
        private void BtnSetQueryParameters_Click(object sender, RoutedEventArgs e)
        {
            if (tbLatitude.Text != "" && tbLongtitude.Text != "" && tbAltitude.Text != "" && cbResponseTime.Text != "" && tbAccuracy.Text != ""
                && cbNumOfAssists.Text != "")
            {
                MainWindow.qParams.latitude = tbLatitude.Text;
                MainWindow.qParams.longtitude = tbLongtitude.Text;
                MainWindow.qParams.altitude = tbAltitude.Text;
                switch(cbResponseTime.Text)
                {
                    case "1":
                        MainWindow.qParams.responseTime = "0";
                        break;
                    case "2":
                        MainWindow.qParams.responseTime = "1";
                        break;
                    case "4":
                        MainWindow.qParams.responseTime = "2";
                        break;
                    case "8":
                        MainWindow.qParams.responseTime = "3";
                        break;
                    case "16":
                        MainWindow.qParams.responseTime = "4";
                        break;
                    case "32":
                        MainWindow.qParams.responseTime = "5";
                        break;
                    case "64":
                        MainWindow.qParams.responseTime = "6";
                        break;
                    case "128":
                        MainWindow.qParams.responseTime = "7";
                        break;
                }
                MainWindow.qParams.accuracy = tbAccuracy.Text;
                MainWindow.qParams.numOfAssists = cbNumOfAssists.Text;
                this.Close();
            }
            else
                MessageBox.Show("All fields are mandatory!");
        }
        private static bool IsInt(string text)
        {
            return _regexInt.IsMatch(text);
        }
        private static bool IsDouble(string text)
        {
            return _regexDouble.IsMatch(text);
        }
        private void PreviewAccuracy(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsInt(e.Text);
        }
        private void PreviewLatitude(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsDouble(e.Text);
        }
        private void PreviewLongtitude(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsDouble(e.Text);
        }
        private void PreviewAltitude(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsInt(e.Text);
        }
        private void PreviewRefreshAlm(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsDouble(e.Text);
        }
        private void PreviewRefreshEph(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsDouble(e.Text);
        }
    }
}
