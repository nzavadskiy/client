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
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Reactive.Linq;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Timers;
using Microsoft.Maps.MapControl.WPF;
using System.Text.RegularExpressions;
using System.Globalization;
using Newtonsoft.Json;

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
        SetStartParameters setStartParameters = new SetStartParameters();
        public static List<string> assistInformation;
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
                CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(lvSubscribers.ItemsSource);
                PropertyGroupDescription groupDescriptionBsName = new PropertyGroupDescription("bsName");
                view.GroupDescriptions.Add(groupDescriptionBsName);

                lvGeolocation.ItemsSource = geos;
                lvTA.ItemsSource = tas;
                lvCellID.ItemsSource = cellids;
                lvLog.ItemsSource = log;
                cbMapSubs.ItemsSource = subs;
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
        private void GetAndRenewAssistData(Object source, ElapsedEventArgs e)
        {
            App.Current.Dispatcher.BeginInvoke((Action)delegate ()
            {
                assistInformation = SplitAssist.Split(ParseAssist(GetAssistFromInet()));
                foreach (Subscriber s in subs)
                {
                    foreach (string a in assistInformation)
                    {
                        Subscriber s_ = s;
                        s_.assistData = a;
                        Client.SendMessage("3" + s_.Serialize());
                        string res = Client.GetMessage();
                        if (res == "0")
                            Logging(String.Format("Отправлены дополнительные сведения абоненту: IMSI = {0}, IMEI-SV = {1}", s.imsi, s.imeiSV));
                        RenewAssistDataForSub(s);
                    }
                }
            });
        }
        private void RenewAssistDataForSub(Subscriber sub)
        {
           foreach (string a in assistInformation)
           {
                Subscriber s_ = sub;
                s_.assistData = a;
                Client.SendMessage("2" + s_.Serialize());
                string res = Client.GetMessage();
                if (res == "0")
                Logging(String.Format("Отправлены дополнительные сведения абоненту: IMSI = {0}, IMEI-SV = {1}", sub.imsi, sub.imeiSV));
           }
        }
        public void RenewAssistDataPeriodically()
        {
            App.Current.Dispatcher.BeginInvoke((Action)delegate ()
            {                
                assistTimer = new System.Timers.Timer(float.Parse(qParams.timeRefreshEphem, CultureInfo.InvariantCulture) * 1000 * 60 * 60); assistTimer = new System.Timers.Timer(float.Parse(qParams.timeRefreshEphem, CultureInfo.InvariantCulture) * 1000 * 60 * 60);
                assistTimer.Elapsed += GetAndRenewAssistData;
                assistTimer.AutoReset = true;
                assistTimer.Enabled = true;
                Logging("Запущена процедура обновления дополнительной информациии о спутниках");
            });
        }
        public void Logging(string fact)
        {
            tbStatus.Text = fact;
            log.Add(new LogUnit(fact));
        }
        private void Logging(string factToStatus, string factToLog)
        {
            tbStatus.Text = factToStatus;
            log.Add(new LogUnit(factToLog));
        }
        private void BS_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    BaseStation newBS = e.NewItems[0] as BaseStation;
                    log.Add(new LogUnit("Добавлена", newBS));
                    break;                    
                case NotifyCollectionChangedAction.Remove:
                    BaseStation oldBS = e.OldItems[0] as BaseStation;
                    log.Add(new LogUnit("Удалена", oldBS));
                    var subsForRemove = subs.Where(s => s.bsName == oldBS.name).ToList();
                    foreach(Subscriber sub in subsForRemove)
                    {
                        subs.Remove(sub);
                    }
                    break;
            }
        }
        private void Subs_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    Subscriber newSub = e.NewItems[0] as Subscriber;
                    Subscriber s = new Subscriber(newSub.imsi, newSub.imeiSV, newSub.assistData);
                    //RenewAssistDataForSub(s);
                    log.Add(new LogUnit("Добавлен", newSub));
                    break;
                case NotifyCollectionChangedAction.Remove:
                    Subscriber oldSub = e.OldItems[0] as Subscriber;
                    log.Add(new LogUnit("Удален", oldSub));
                    break;
            }
        }
        private void Geos_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    Geolocation newGeo = e.NewItems[0] as Geolocation;
                    log.Add(new LogUnit(newGeo));
                    break;              
            }
        }
        private void TAs_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    TA newTA = e.NewItems[0] as TA;
                    log.Add(new LogUnit(newTA));
                    break;
            }
        }
        private void CellIDs_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    CellID newCellID = e.NewItems[0] as CellID;
                    log.Add(new LogUnit(newCellID));
                    break;
            }
        }
        private void Log_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    LogUnit newUser = e.NewItems[0] as LogUnit;
                    break;
            }
        }
        private string GetAssistFromInet()
        {
            try
            {
                string res;
                using (var client = new WebClient())
                {
                    string q = qParams.GetQuery();
                    res = client.DownloadString(q);
                }
                return res;
            }
            catch(Exception e)
            {
                Logging(e.Message);
                return "";
            }
        }
        public List<string> ParseAssist(string assist)
        {
            List<string> res = new List<string>();
            foreach(Match m in Regex.Matches(assist, @"apdu=([0-9A-Z]+)"))
            {
                res.Add(m.Value.Substring(5));
            }
            return res;
        }
        private void MenuSetParams_Click(object sender, RoutedEventArgs e)
        {
            SetQueryParameters setQueryParameters = new SetQueryParameters
            {
                Owner = this
            };
            setQueryParameters.Show();            
        }
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
        public void GetPeriodicGeolocation(Object source, ElapsedEventArgs e)
        {
            App.Current.Dispatcher.BeginInvoke((Action)delegate ()
            {
                Subscriber sub = new Subscriber();
                sub = (Subscriber)lvSubscribers.SelectedItem;
                sub.assistData = "ms-based gps";
                if (sub != null)
                {
                    Client.SendMessage("2" + sub.Serialize());
                    string res = Client.GetMessage();
                    if (res == "0")
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
                    Client.SendMessage("5" + sub.Serialize());
                    string res = Client.GetMessage();
                    if (res == "0")
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
                    Client.SendMessage("6" + sub.Serialize());
                    string res = Client.GetMessage();
                    if (res == "0")
                        Logging(String.Format("Отправлен запрос на определение местоположения абоненту: IMSI = {0}, IMEI_SV = {1}", sub.imsi, sub.imeiSV));
                }
                else
                    Logging("Абонент отключился");
            });
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