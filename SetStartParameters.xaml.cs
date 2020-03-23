using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
        }
        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainwindow = new MainWindow();
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
