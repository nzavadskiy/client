using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Reactive.Linq;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Timers;
using Microsoft.Maps.MapControl.WPF;
using System.Text.RegularExpressions;
using System.Globalization;

namespace client
{
    public partial class MainWindow : Window
    {
        public static IPAddress localIP;
        public static int localPort;
        public static int renewAssistInterval;

        public static int latencyTime = 1;
        public static Thread serverThread;
        public static Thread assistThread;
        public static ObservableCollection<BaseStation> bss;
        public static ObservableCollection<Subscriber> subs;
        public static ObservableCollection<Geolocation> geos;
        public static ObservableCollection<TA> tas;
        public static ObservableCollection<CellID> cellids;
        public static ObservableCollection<LogUnit> log;
        public static QueryParameters qParams;
        public static System.Timers.Timer aTimer;
        System.Timers.Timer assistTimer;
        public static bool isWindowActive = false;
        public static bool isServerStarted = false;
        SetStartParameters setStartParameters = new SetStartParameters();
        public static List<string> assistInformation;
        public static List<Client> clients;

        CollectionView bsView;
        CollectionView subView;
        CollectionView geoView;
        CollectionView taView;
        CollectionView cellIDView;
    }
}
