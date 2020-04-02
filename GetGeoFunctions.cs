using System;
using System.Windows;

namespace client
{
    public partial class MainWindow : Window
    {
        private void GetGeolocation(string assist)
        {
            GetGeolocation(1, assist);
        }
        private void GetGeolocation(int num, string assist)
        {
            Subscriber sub = new Subscriber();
            sub = (Subscriber)lvSubscribers.SelectedItem;
            sub.assistData = assist;
            string msg = sub.Serialize();
            Client.SendMessage(num.ToString() + msg);
            string res = Client.GetMessage();
            if (res == "90")
                Logging(String.Format("Отправлен запрос на определение местоположения абоненту: IMSI = {0}, IMEI_SV = {1}", sub.imsi, sub.imeiSV));
        }
        private void GetGeolocationAll(string assist)
        {
            GetGeolocationAll(1, assist);
        }
        private void GetGeolocationAll(int num, string assist)
        {
            foreach (Subscriber sub in subs)
            {
                sub.assistData = assist;
                Client.SendMessage(num.ToString() + sub.Serialize());
                string res = Client.GetMessage();
                if (res == "90")
                    Logging(String.Format("Отправлен запрос на определение местоположения абоненту: IMSI = {0}, IMEI_SV = {1}", sub.imsi, sub.imeiSV));
            }
        }
        private void MiGetMsbGPS(object sender, RoutedEventArgs e)
        {
            GetGeolocation("ms-based gps");
        }
        private void MiGetMsaGPS(object sender, RoutedEventArgs e)
        {
            GetGeolocation("ms-assisted gps");
        }
        private void MiGetMsbEOTD(object sender, RoutedEventArgs e)
        {
            GetGeolocation("ms-based e-otd");
        }
        private void MiGetMsaEOTD(object sender, RoutedEventArgs e)
        {
            GetGeolocation("ms-assisted e-otd");
        }
        private void MiGetTA(object sender, RoutedEventArgs e)
        {
            GetGeolocation(2, "");
        }
        private void MiGetCellID(object sender, RoutedEventArgs e)
        {
            GetGeolocation(3, "");
        }
        private void MiGetMsbGPSAll(object sender, RoutedEventArgs e)
        {
            GetGeolocationAll("ms-based gps");
        }
        private void MiGetMsaGPSAll(object sender, RoutedEventArgs e)
        {
            GetGeolocationAll("ms-assisted gps");
        }
        private void MiGetMsbEOTDAll(object sender, RoutedEventArgs e)
        {
            GetGeolocationAll("ms-based e-otd");
        }
        private void MiGetMsaEOTDAll(object sender, RoutedEventArgs e)
        {
            GetGeolocationAll("ms-assisted e-otd");
        }
        private void MiGetTAAll(object sender, RoutedEventArgs e)
        {
            GetGeolocationAll(2, "");
        }
        private void MiGetCellIDAll(object sender, RoutedEventArgs e)
        {
            GetGeolocationAll(6, "");
        }
    }
}