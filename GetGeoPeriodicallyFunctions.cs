using System;
using System.Windows;
using System.Timers;

namespace client
{
    public partial class MainWindow : Window
    {
        private void MiGetPerMsbGPS(object sender, RoutedEventArgs e)
        {
            PeriodicQueryWindow periodicQueryWindow = new PeriodicQueryWindow
            {
                Owner = this
            };
            periodicQueryWindow.Show();
            aTimer = new System.Timers.Timer(latencyTime * 1000);
            aTimer.Elapsed += GetPeriodicGeolocation;
            aTimer.AutoReset = true;
            //aTimer.Enabled = true;
            Logging("Запущена процедура периодического получения местоположения");
        }
        private void MiGetPerMsaGPS(object sender, RoutedEventArgs e)
        {
            PeriodicQueryWindow periodicQueryWindow = new PeriodicQueryWindow
            {
                Owner = this
            };
            periodicQueryWindow.Show();
            aTimer = new System.Timers.Timer(latencyTime * 1000);
            aTimer.Elapsed += GetPeriodicGeolocation;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;
            Logging("Запущена процедура периодического получения местоположения");
        }
        private void MiGetPerMsbEOTD(object sender, RoutedEventArgs e)
        {
            PeriodicQueryWindow periodicQueryWindow = new PeriodicQueryWindow
            {
                Owner = this
            };
            periodicQueryWindow.Show();
            aTimer = new System.Timers.Timer(latencyTime * 1000);
            aTimer.Elapsed += GetPeriodicGeolocation;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;
            Logging("Запущена процедура периодического получения местоположения");
        }
        private void MiGetPerMsaEOTD(object sender, RoutedEventArgs e)
        {
            PeriodicQueryWindow periodicQueryWindow = new PeriodicQueryWindow
            {
                Owner = this
            };
            periodicQueryWindow.Show();
            aTimer = new System.Timers.Timer(latencyTime * 1000);
            aTimer.Elapsed += GetPeriodicGeolocation;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;
            Logging("Запущена процедура периодического получения местоположения");
        }
        private void MiGetPerTA(object sender, RoutedEventArgs e)
        {
            PeriodicQueryWindow periodicQueryWindow = new PeriodicQueryWindow
            {
                Owner = this
            };
            periodicQueryWindow.Show();
            aTimer = new System.Timers.Timer(latencyTime * 1000);
            aTimer.Elapsed += GetPeriodicGeolocationTA;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;
            Logging("Запущена процедура периодического получения местоположения");
        }
        private void MiGetPerCellID(object sender, RoutedEventArgs e)
        {
            PeriodicQueryWindow periodicQueryWindow = new PeriodicQueryWindow
            {
                Owner = this
            };
            periodicQueryWindow.Show();
            aTimer = new System.Timers.Timer(latencyTime * 1000);
            aTimer.Elapsed += GetPeriodicGeolocationCellID;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;
            Logging("Запущена процедура периодического получения местоположения");
        }
        private void MiStopGeoPeriod_Click(object sender, RoutedEventArgs e)
        {
            if (assistTimer.Enabled)
            {
                aTimer.Enabled = false;
                Logging("Остановлена процедура периодического получения местоположения");
            }
            else
                Logging("Таймер не задан");
        }
        public void GetPeriodicGeolocation(Object source, ElapsedEventArgs e)
        {
            App.Current.Dispatcher.BeginInvoke((Action)delegate ()
            {
                Subscriber sub = new Subscriber();
                sub = (Subscriber)lvSubscribers.SelectedItem;
                sub.assistData = "ms-based gps";
                if (sub != null)
                {
                    SendReceiveMessage(sub.bsName, "2" + sub.Serialize());
                    Logging(String.Format("Отправлен запрос на определение местоположения абоненту: IMSI = {0}, IMEI_SV = {1}", sub.imsi, sub.imeiSV));
                }
                else
                    Logging("Абонент отключился");
            });
        }
        private void GetPeriodicGeolocationTA(Object source, ElapsedEventArgs e)
        {
            App.Current.Dispatcher.BeginInvoke((Action)delegate ()
            {
                Subscriber sub = new Subscriber();
                sub = (Subscriber)lvSubscribers.SelectedItem;
                sub.assistData = "";
                if (sub != null)
                {
                    SendReceiveMessage(sub.bsName, "5" + sub.Serialize());
                    Logging(String.Format("Отправлен запрос на определение местоположения абоненту: IMSI = {0}, IMEI_SV = {1}", sub.imsi, sub.imeiSV));
                }
                else
                    Logging("Абонент отключился");
            });
        }
        private void GetPeriodicGeolocationCellID(Object source, ElapsedEventArgs e)
        {
            App.Current.Dispatcher.BeginInvoke((Action)delegate ()
            {
                Subscriber sub = new Subscriber();
                sub = (Subscriber)lvSubscribers.SelectedItem;
                sub.assistData = "";
                if (sub != null)
                {
                    SendReceiveMessage(sub.bsName, "6" + sub.Serialize());
                    Logging(String.Format("Отправлен запрос на определение местоположения абоненту: IMSI = {0}, IMEI_SV = {1}", sub.imsi, sub.imeiSV));
                }
                else
                {
                    Logging("Абонент отключился");
                }
            });
        }
    }
}