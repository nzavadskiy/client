using System;
using System.Collections.Generic;
using System.Globalization;
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
            tbAccuracy.Text = MainWindow.qParams.accuracy.ToString();
            cbNumOfAssists.SelectedIndex = MainWindow.numOfAssistsValues[MainWindow.qParams.numOfAssists];
            tbLatitude.Text = MainWindow.qParams.latitude.ToString(CultureInfo.InvariantCulture);
            tbLongtitude.Text = MainWindow.qParams.longtitude.ToString(CultureInfo.InvariantCulture);
            tbAltitude.Text = MainWindow.qParams.altitude.ToString();
            tbResponseTime.Text = MainWindow.qParams.responseTime.ToString();
            tbRenewAssistInterval.Text = MainWindow.qParams.refreshInterval.ToString();
        }
        private void BtnSetQueryParameters_Click(object sender, RoutedEventArgs e)
        {
            if (tbLatitude.Text != "" && tbLongtitude.Text != "" && tbAltitude.Text != "" && tbResponseTime.Text != "" && tbAccuracy.Text != ""
                && cbNumOfAssists.Text != "")
            {
                MainWindow.qParams.latitude = double.Parse(tbLatitude.Text, CultureInfo.InvariantCulture);
                MainWindow.qParams.longtitude = double.Parse(tbLongtitude.Text, CultureInfo.InvariantCulture);
                MainWindow.qParams.altitude = int.Parse(tbAltitude.Text);
                MainWindow.qParams.responseTime = int.Parse(tbResponseTime.Text);
                MainWindow.qParams.accuracy = int.Parse(tbAccuracy.Text);
                MainWindow.qParams.numOfAssists = int.Parse(cbNumOfAssists.Text);
                MainWindow.qParams.refreshInterval = int.Parse(tbRenewAssistInterval.Text);
                this.Close();
            }
            else
            {
                MessageBox.Show("All fields are mandatory!");
            }
        }
        private static bool IsInt(string text)
        {
            return _regexInt.IsMatch(text);
        }
        private static bool IsDouble(string text)
        {
            return _regexDouble.IsMatch(text);
        }
        private void TbResponseTime_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsInt(e.Text);
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
        private void TbRenewAssistInterval_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsInt(e.Text);
        }
        private void TbAccuracy_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (tbAccuracy.Text != "")
            {
                int value = int.Parse(tbAccuracy.Text);
                if (value >= 0 && value <= 127)
                {
                    tbAccuracy.Background = Brushes.LightGreen;
                    int metersValue = (int)(10 * (Math.Pow(1.1, value) - 1));
                    tbAccuracyMeters.Text = "Метры: " + metersValue.ToString();
                }
                else
                {
                    tbAccuracy.Background = Brushes.Red;
                    tbAccuracyMeters.Text = "Метры: ";
                }
            }
            else
            {
                tbAccuracy.Background = Brushes.Red;
                tbAccuracyMeters.Text = "Метры: ";
            }
        }

        private void TbResponseTime_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (tbResponseTime.Text != "")
            {
                int value = int.Parse(tbResponseTime.Text);
                if (value >= 0 && value <= 7)
                {
                    tbResponseTime.Background = Brushes.LightGreen;
                    int metersValue = (int)(Math.Pow(2, value));
                    tbResponseTimeSeconds.Text = "Секунды: " + metersValue.ToString();
                }
                else
                {
                    tbResponseTime.Background = Brushes.Red;
                    tbResponseTimeSeconds.Text = "Секунды: ";
                }
            }
            else
            {
                tbResponseTime.Background = Brushes.Red;
                tbResponseTimeSeconds.Text = "Секунды: ";
            }
        }

       
    }
}
