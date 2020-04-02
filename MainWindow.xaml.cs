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

namespace client
{    
    public static class Client
    {
        public static TcpClient client;
        public static IPAddress ip { get; set; }
        public static NetworkStream stream { get; set; }
        public static void Start() { }
        public static int SendMessage(string message)
        {
            Byte[] data = Encoding.ASCII.GetBytes(message);
            stream.Write(data, 0, data.Length);
            return 0;
        }
        public static string GetMessage()
        {
            byte[] data = new Byte[1024];
            int bytes = stream.Read(data, 0, data.Length);
            return Encoding.ASCII.GetString(data, 0, bytes);
        }
        public static void CloseClient()
        {
            stream.Close();
            client.Close();
        }
    }
   
    public class QueryParameters
    {
        public string responseTime; 
        public string accuracy;
        public string numOfAssists;
        public string latitude;
        public string longtitude;
        public string altitude;
        public string timeRefreshAmanac;
        public string timeRefreshEphem;
        private const string almanacURL = "687474703a2f2f7777772e6e617663656e2e757363672e676f762f3f706167654e616d653d63757272656e74416c6d616e616326666f726d61743d79756d61";
        private const string ephemerisURL = "6674703a2f2f6674702e7472696d626c652e636f6d2f7075622f6570682f437572526e784e2e6e6176";
        public QueryParameters() { }
        public QueryParameters(string rt, string acc, string num, string lat, string lon, string alt, string timeA, string timeE)
        {
            responseTime = rt;
            accuracy = acc;
            numOfAssists = num;
            latitude = lat;
            longtitude = lon;
            altitude = alt;
            timeRefreshAmanac = timeA;
            timeRefreshEphem = timeE;
        }
        public string GetQuery()
        {
            return String.Format(@"http://192.168.70.132/cgi-bin/rrlpserver.cgi?GSM.RRLP.ACCURACY={1}&GSM.RRLP.RESPONSETIME={0}&GSM.RRLP.ALMANAC.URL={8}&GSM.RRLP.EPHEMERIS.URL={9}&GSM.RRLP.ALMANAC.REFRESH.TIME={6}&GSM.RRLP.EPHEMERIS.REFRESH.TIME={7}&GSM.RRLP.SEED.LATITUDE={3}.0&GSM.RRLP.SEED.LONGITUDE={4}.0&GSM.RRLP.SEED.ALTITUDE={5}&GSM.RRLP.ALMANAC.ASSIST.PRESENT=0&GSM.RRLP.EPHEMERIS.ASSIST.COUNT={2}&query=assist",
                responseTime, accuracy, numOfAssists, latitude, longtitude, altitude, timeRefreshAmanac, timeRefreshEphem, almanacURL, ephemerisURL);
        }
    }
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            if (!isWindowActive)
            {                
                setStartParameters.Show();
                Hide();
                isWindowActive = true;
            }
        }
        private void MenuStartTestClient_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.PresentationTraceSources.SetTraceLevel(lvBaseStations.ItemContainerGenerator, System.Diagnostics.PresentationTraceLevel.High);
                System.Diagnostics.PresentationTraceSources.SetTraceLevel(lvGeolocation.ItemContainerGenerator, System.Diagnostics.PresentationTraceLevel.High);
                System.Diagnostics.PresentationTraceSources.SetTraceLevel(lvSubscribers.ItemContainerGenerator, System.Diagnostics.PresentationTraceLevel.High);
                qParams = new QueryParameters("7", "7", "15", "55", "37", "0", "24", "0.1");
                bss = new ObservableCollection<BaseStation>();
                subs = new ObservableCollection<Subscriber>();
                geos = new ObservableCollection<Geolocation>();
                tas = new ObservableCollection<TA>();
                cellids = new ObservableCollection<CellID>();
                log = new ObservableCollection<LogUnit>();
                bss.CollectionChanged += BS_CollectionChanged;
                subs.CollectionChanged += Subs_CollectionChanged;
                geos.CollectionChanged += Geos_CollectionChanged;
                tas.CollectionChanged += TAs_CollectionChanged;
                cellids.CollectionChanged += CellIDs_CollectionChanged;
                log.CollectionChanged += Log_CollectionChanged;
                lvBaseStations.ItemsSource = bss;
                lvSubscribers.ItemsSource = subs;

                lvGeolocation.ItemsSource = geos;
                lvTA.ItemsSource = tas;
                lvCellID.ItemsSource = cellids;
                lvLog.ItemsSource = log;
                cbMapSubs.ItemsSource = subs;

                bsView = (CollectionView)CollectionViewSource.GetDefaultView(lvSubscribers.ItemsSource);
                bsView.GroupDescriptions.Add(new PropertyGroupDescription("bsName"));
                bsView.Filter = BSFilter;
                subView = (CollectionView)CollectionViewSource.GetDefaultView(lvSubscribers.ItemsSource);
                subView.Filter = SubFilter;
                geoView = (CollectionView)CollectionViewSource.GetDefaultView(lvGeolocation.ItemsSource);
                geoView.Filter = GeoFilter;
                taView = (CollectionView)CollectionViewSource.GetDefaultView(lvTA.ItemsSource);
                taView.Filter = TAFilter;
                cellIDView = (CollectionView)CollectionViewSource.GetDefaultView(lvCellID.ItemsSource);
                cellIDView.Filter = CellIDFilter;

                serverThread = new Thread(new ThreadStart(AsynchronousSocketListener.StartListening));
                serverThread.Start();

                //assistInformation = SplitAssist.Split(ParseAssist(GetAssistFromInet())); // только с подключением к rrlp-серверу
                //assistThread = new Thread(new ThreadStart(RenewAssistDataPeriodically));
                //assistThread.Start();
            }
            catch (Exception e1)
            {
                Logging("Ошибка " + e1.HResult.ToString(), e1.Message);
            }
        }
        private void MenuSetParams_Click(object sender, RoutedEventArgs e)
        {
            SetQueryParameters setQueryParameters = new SetQueryParameters
            {
                Owner = this
            };
            setQueryParameters.Show();            
        }
        private void BtnMapSetSub_click(object sender, RoutedEventArgs e)
        {
            if(cbMapSubs.SelectedValue != null)
            {
                //myMap.Children.Clear();
                foreach (Geolocation g in geos)
                {
                    if (g.imsi == cbMapSubs.SelectedValue.ToString())
                    {
                        //Location pinLocation = myMap.ViewportPointToLocation(new Point(g.latitude, g.longtitude));
                        Location pinLocation = new Location(g.latitude, g.longtitude);
                        Pushpin pin = new Pushpin();
                        pin.Location = pinLocation;
                        ToolTip CoordinateTip = new ToolTip();
                        CoordinateTip.Content = pinLocation.ToString();
                        pin.ToolTip = CoordinateTip;
                        myMap.Children.Add(pin);
                    }
                }
            }
        }        
        public static void AddNewSub(SubscribersChange Subs)
        {
            App.Current.Dispatcher.BeginInvoke((Action)delegate ()
            {
                subs.Add(Subs.NewSub);     
            });
        }
        public static void RemoveSub(SubscribersChange Subs)
        {
            App.Current.Dispatcher.BeginInvoke((Action)delegate ()
            {
                subs.Remove(Subs.NewSub);
            });
        }
        public static void AddNewBS(BaseStationChange BSs)
        {
            App.Current.Dispatcher.BeginInvoke((Action)delegate ()
            {
                bss.Add(BSs.NewBS);
            });
        }
        public static void RemoveBS(BaseStationChange BSs)
        {
            App.Current.Dispatcher.BeginInvoke((Action)delegate ()
            {
                bss.Remove(BSs.NewBS);
            });
        }
        public static void AlterBS(BaseStationChange BSs)
        {
            App.Current.Dispatcher.BeginInvoke((Action)delegate ()
            {
                foreach(BaseStation bs in bss.Where(b => b.name == BSs.NewBS.name))
                {
                    if (BSs.NewBS.mcc != "")
                        bs.mcc = BSs.NewBS.mcc;
                    if (BSs.NewBS.mnc != "")
                        bs.mnc = BSs.NewBS.mnc;
                    if (BSs.NewBS.lac != "")
                        bs.lac = BSs.NewBS.lac;
                    if (BSs.NewBS.cellid != "")
                        bs.cellid = BSs.NewBS.cellid;
                    if (BSs.NewBS.lat != "")
                        bs.lat = BSs.NewBS.lat;
                    if (BSs.NewBS.lon != "")
                        bs.lon = BSs.NewBS.lon;
                    if (BSs.NewBS.antenna != "")
                        bs.antenna = BSs.NewBS.antenna;
                }
            });
        }
        public static void RemoveAllBS(BaseStationChange BSs)
        {
            App.Current.Dispatcher.BeginInvoke((Action)delegate ()
            {
                var bssToRemove = bss.Where(b => b.ip == BSs.NewBS.ip && b.port == BSs.NewBS.port).ToList();
                foreach (var bs in bssToRemove)
                    bss.Remove(bs);
            });
        }
        public static void AddNewGeo(GeolocationChange Geos)
        {
            App.Current.Dispatcher.BeginInvoke((Action)delegate ()
            {
                geos.Add(Geos.NewGeo);
            });
        }
        public static void AddNewTA(TAChange TAs)
        {
            App.Current.Dispatcher.BeginInvoke((Action)delegate ()
            {
                tas.Add(TAs.NewTA);
            });
        }
        public static void AddNewCellID(CellIDChange CellIDs)
        {
            App.Current.Dispatcher.BeginInvoke((Action)delegate ()
            {
                CellID cellid = CellIDs.NewCellID;
                foreach(Subscriber sub in subs.Where(s => s.imsi == cellid.imsi && s.imeiSV == cellid.imeiSV))
                {
                    cellid.bsName = sub.bsName;
                    foreach(BaseStation bs in bss.Where(b => b.name == sub.bsName))
                    {
                        cellid.lat = bs.lat;
                        cellid.lon = bs.lon;
                    }
                }
                cellids.Add(cellid);
            });
        }
        public static void AddNewLog(LogChange Log)
        {
            App.Current.Dispatcher.BeginInvoke((Action)delegate ()
            {
                log.Add(Log.NewLog);
            });
        }
        private void FormClosing(object sender, CancelEventArgs e)
        {
            AsynchronousSocketListener.StopListening();
            Application.Current.Shutdown();
        }        
    }    
}