using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
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
    public partial class SetStartParameters : Window
    {
        public SetStartParameters()
        {
            InitializeComponent();
            string content = File.ReadAllText("..\\..\\configs\\network.json");
            dynamic networkConf = JsonConvert.DeserializeObject(content);
            if (networkConf.ContainsKey("ip"))
            {
                tbLocalIP.Text = networkConf.ip;
            }
            if (networkConf.ContainsKey("port"))
            {
                tbLocalPort.Text = networkConf.port;
            }
        }
        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainwindow = new MainWindow();
            if (!System.Net.IPAddress.TryParse(tbLocalIP.Text, out MainWindow.localIP))
            {
                MessageBox.Show("Неверный формат IP адреса");
                return;
            }
            if (!int.TryParse(tbLocalPort.Text, out MainWindow.localPort))
            {
                MessageBox.Show("Неверный формат номера порта");
                return;
            }
            mainwindow.Show();
            MainWindow.isWindowActive = true;
            Hide();
        }
        private void FormClosing(object sender, CancelEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
