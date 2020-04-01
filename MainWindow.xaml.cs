using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Windows.Threading;
using System.Reactive;
using System.Reactive.Subjects;
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
    [DataContract]
    public class BaseStation
    {
        [DataMember]
        public string name { get; set; }
        [DataMember]
        public string mcc { get; set; }
        [DataMember]
        public string mnc { get; set; }
        [DataMember]
        public string cellid { get; set; }
        [DataMember]
        public string lac { get; set; }
        [DataMember]
        public string lat { get; set; }
        [DataMember]
        public string lon { get; set; }
        [DataMember]
        public string antenna { get; set; }
        [DataMember]
        public string ip { get; set; }
        [DataMember]
        public string port { get; set; }
        
        public BaseStation() { }
        public BaseStation(string name, string mcc, string mnc, string cellid, string lac, string lat, string lon, string antenna, string ip, string port)
        {
            this.name = name;
            this.mcc = mcc;
            this.mnc = mnc;
            this.cellid = cellid;
            this.lac = lac;
            this.lat = lat;
            this.lon = lon;
            this.antenna = antenna;
            this.ip = ip;
            this.port = port;
        }
        public string Serialize()
        {
            DataContractJsonSerializer jsonFormatter = new DataContractJsonSerializer(typeof(BaseStation));
            MemoryStream stream = new MemoryStream();
            jsonFormatter.WriteObject(stream, this);
            stream.Position = 0;
            StreamReader sr = new StreamReader(stream);
            return sr.ReadToEnd();
        }
        public static BaseStation Deserialize(string jsonStr)
        {
            DataContractJsonSerializer jsonFormatter = new DataContractJsonSerializer(typeof(BaseStation));
            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(jsonStr));
            return (BaseStation)jsonFormatter.ReadObject(stream);
        }
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            BaseStation c = (BaseStation)obj;
            return name == c.name;
        }
        public void GetBSCoord()
        {
            string bsCoordResStr;
            using (var client = new WebClient())
            {
                bsCoordResStr = client.DownloadString(String.Format("https://api.mylnikov.org/geolocation/cell?v=1.1&data=open&mcc={0}&mnc={1}&lac={2}&cellid={3}", mcc, mnc, lac, cellid));
            }
            dynamic bsCoordRes = JsonConvert.DeserializeObject(bsCoordResStr);
            if (bsCoordRes.ContainsKey("data"))
            {
                if (bsCoordRes.data.ContainsKey("lat"))
                {
                    lat = bsCoordRes.data.lat;
                }
                if (bsCoordRes.data.ContainsKey("lon"))
                {
                    lon = bsCoordRes.data.lon;
                }
            }
        }

        public override int GetHashCode()
        {
            return 363513814 + EqualityComparer<string>.Default.GetHashCode(name);
        }
    }
    [DataContract]
    public class Subscriber
    {
        [DataMember]
        public string imsi { get; set; }
        [DataMember]
        public string imeiSV { get; set; }
        [DataMember]
        public string subName { get; set; }
        [DataMember]
        public string assistData { get; set; }
        [DataMember]
        public string bsName { get; set; }
        public Subscriber() { }
        public Subscriber(string imsi, string imeiSV)
        {
            this.imsi = imsi;
            this.imeiSV = imeiSV;
        }
        public Subscriber(string imsi, string imeiSV, string assistData)
        {
            this.imsi = imsi;
            this.imeiSV = imeiSV;
            this.assistData = assistData;
        }
        public string Serialize()
        {
            DataContractJsonSerializer jsonFormatter = new DataContractJsonSerializer(typeof(Subscriber));
            MemoryStream stream = new MemoryStream();
            jsonFormatter.WriteObject(stream, this);
            stream.Position = 0;
            StreamReader sr = new StreamReader(stream);
            return sr.ReadToEnd();
        }
        public static Subscriber Deserialize(string jsonStr)
        {
            DataContractJsonSerializer jsonFormatter = new DataContractJsonSerializer(typeof(Subscriber));
            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(jsonStr));
            return (Subscriber)jsonFormatter.ReadObject(stream);
        } 
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            Subscriber c = (Subscriber)obj;
            return imsi == c.imsi && imeiSV == c.imeiSV;
        }
        public override int GetHashCode()
        {
            var hashCode = -1238749623;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(imsi);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(imeiSV);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(assistData);
            return hashCode;
        }
        public void ParseClassmark()
        {
            string cm = assistData;
            assistData = "";
            if (cm[0] == '1')
                assistData += " MS-Based E-OTD";
            if (cm[1] == '1')
                assistData += " MS-Assisted E-OTD";
            if (cm[2] == '1')
                assistData += " MS-Based GPS";
            if (cm[3] == '1')
                assistData += " MS-Assisted GPS";
        }
    }
    [DataContract]
    public class Geolocation
    {
        [DataMember]
        public string imsi { get; set; }
        [DataMember]
        public string imeiSV { get; set; }
        [DataMember]
        public double latitude { get; set; }
        [DataMember]
        public double longtitude { get; set; }
        [DataMember]
        public string date { get; set; }
        public Geolocation(string imsi, string imeiSV, double latitude, double longtitude, string date)
        {
            this.imsi = imsi;
            this.imeiSV = imeiSV;
            this.latitude = latitude;
            this.longtitude = longtitude;
            this.date = date;
        }        
        public string Serialize()
        {
            DataContractJsonSerializer jsonFormatter = new DataContractJsonSerializer(typeof(Geolocation));
            MemoryStream stream = new MemoryStream();
            jsonFormatter.WriteObject(stream, this);
            stream.Position = 0;
            StreamReader sr = new StreamReader(stream);
            return sr.ReadToEnd();
        }
        public static Geolocation Deserialize(string jsonStr)
        {
            DataContractJsonSerializer jsonFormatter = new DataContractJsonSerializer(typeof(Geolocation));
            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(jsonStr));
            return (Geolocation)jsonFormatter.ReadObject(stream);
        }
    }
    [DataContract]
    public class TA
    {
        [DataMember]
        public string imsi { get; set; }
        [DataMember]
        public string imeiSV { get; set; }
        [DataMember]
        public string ta { get; set; }
        [DataMember]
        public string lev1 { get; set; }
        [DataMember]
        public string lev2 { get; set; }
        [DataMember]
        public string lev3 { get; set; }
        [DataMember]
        public string lev4 { get; set; }
        [DataMember]
        public string lev5 { get; set; }
        [DataMember]
        public string lev6 { get; set; }
        [DataMember]
        public string lev7 { get; set; }
        public string Serialize()
        {
            DataContractJsonSerializer jsonFormatter = new DataContractJsonSerializer(typeof(TA));
            MemoryStream stream = new MemoryStream();
            jsonFormatter.WriteObject(stream, this);
            stream.Position = 0;
            StreamReader sr = new StreamReader(stream);
            return sr.ReadToEnd();
        }
        public static TA Deserialize(string jsonStr)
        {
            DataContractJsonSerializer jsonFormatter = new DataContractJsonSerializer(typeof(TA));
            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(jsonStr));
            return (TA)jsonFormatter.ReadObject(stream);
        }
    }
    [DataContract]
    public class CellID
    {
        [DataMember]
        public string imsi { get; set; }
        [DataMember]
        public string imeiSV { get; set; }
        [DataMember]
        public string bsName { get; set; }
        [DataMember]
        public string lat { get; set; }
        [DataMember]
        public string lon { get; set; }
        [DataMember]
        public string dist { get; set; }

        public new string Serialize()
        {
            DataContractJsonSerializer jsonFormatter = new DataContractJsonSerializer(typeof(CellID));
            MemoryStream stream = new MemoryStream();
            jsonFormatter.WriteObject(stream, this);
            stream.Position = 0;
            StreamReader sr = new StreamReader(stream);
            return sr.ReadToEnd();
        }
        public new static CellID Deserialize(string jsonStr)
        {
            DataContractJsonSerializer jsonFormatter = new DataContractJsonSerializer(typeof(CellID));
            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(jsonStr));
            return (CellID)jsonFormatter.ReadObject(stream);
        }

    }
    public class BaseStationChange
    {
        public string AddRemove { get; set; }
        public BaseStation NewBS { get; set; }
    }
    public class SubscribersChange
    {
        public string AddRemove { get; set; }
        public Subscriber NewSub { get; set; }
    }
    public class GeolocationChange
    {
        public Geolocation NewGeo { get; set; }
    }
    public class TAChange
    {
        public TA NewTA { get; set; }
    }
    public class CellIDChange
    {
        public CellID NewCellID { get; set; }
    }
    public class LogChange
    {
        public LogUnit NewLog { get; set; }
    }
    public class LogUnit
    {
        public string time { get; set; }
        public string fact { get; set; }
        public LogUnit() { }
        public LogUnit(DateTime time, string fact)
        {
            this.time = time.ToLongTimeString();
            this.fact = fact;
        }
        public LogUnit(string fact)
        {
            this.fact = fact;
            this.time = DateTime.Now.ToLongTimeString();
        }
        public LogUnit(string fact, BaseStation bs)
        {
            this.time = DateTime.Now.ToLongTimeString();
            this.fact = String.Format("{0} базовая станция:Имя = {1}, MCC = {2}, MNC = {3}, CellID = {4}, LAC = {5}",
                fact, bs.name, bs.mcc, bs.mnc, bs.cellid, bs.lac);
        }
        public LogUnit(string fact, Subscriber sub)
        {
            this.time = DateTime.Now.ToLongTimeString();
            this.fact = String.Format("{0} абонент: IMSI = {1}, IMEI-SV = {2}", fact, sub.imsi, sub.imeiSV);
        }
        public LogUnit(Geolocation geo)
        {
            this.time = DateTime.Now.ToLongTimeString();
            this.fact = String.Format("Добавлено местоположение: IMSI = {0}, IMEI-SV = {1}, latitude = {2}, longtitude = {3}", geo.imsi, geo.imeiSV, geo.latitude, geo.longtitude);
        }
        public LogUnit(TA _ta)
        {
            this.time = DateTime.Now.ToLongTimeString();
            this.fact = String.Format("Добавлены данные о местоположении с использованием TA: IMSI = {0}, IMEI-SV = {1}", _ta.imsi, _ta.imeiSV);
        }
        public LogUnit(CellID _cellid)
        {
            this.time = DateTime.Now.ToLongTimeString();
            this.fact = String.Format("Добавлено данные о местоположении с использованием Cell ID: IMSI = {0}, IMEI-SV = {1}", _cellid.imsi, _cellid.imeiSV);
        }
    }
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
    public class StateObject // State object for reading client data asynchronously
    {
        // Client  socket.  
        public Socket workSocket = null;
        // Size of receive buffer.  
        public const int BufferSize = 1024;
        // Receive buffer.  
        public byte[] buffer = new byte[BufferSize];
        // Received data string.  
        public StringBuilder sb = new StringBuilder();
    }
    public class AsynchronousSocketListener
    {
        public static ISubject<BaseStationChange> bsChange;
        public static ISubject<SubscribersChange> subChange;
        public static ISubject<GeolocationChange> geoChange;
        public static ISubject<TAChange> taChange;
        public static ISubject<CellIDChange> cellidChange;
        public static ISubject<LogChange> logChange;

        public static volatile bool listening = true;

        // Thread signal.  
        public static ManualResetEvent allDone = new ManualResetEvent(false);

        public AsynchronousSocketListener()
        {
        }

        public static void StartListening()
        {            
            bsChange = new Subject<BaseStationChange>();
            subChange = new Subject<SubscribersChange>();
            geoChange = new Subject<GeolocationChange>();
            taChange = new Subject<TAChange>();
            cellidChange = new Subject<CellIDChange>();
            logChange = new Subject<LogChange>();
            subChange.Where(c => c.AddRemove == "+").Subscribe(MainWindow.AddNewSub);
            subChange.Where(c => c.AddRemove == "-").Subscribe(MainWindow.RemoveSub);
            bsChange.Where(c => c.AddRemove == "+").Subscribe(MainWindow.AddNewBS);
            bsChange.Where(c => c.AddRemove == "-").Subscribe(MainWindow.RemoveBS);
            bsChange.Where(c => c.AddRemove == "+-").Subscribe(MainWindow.AlterBS);
            bsChange.Where(c => c.AddRemove == "-all").Subscribe(MainWindow.RemoveAllBS);
            geoChange.Subscribe(MainWindow.AddNewGeo);
            taChange.Subscribe(MainWindow.AddNewTA);
            cellidChange.Subscribe(MainWindow.AddNewCellID);
            logChange.Subscribe(MainWindow.AddNewLog);

            // Establish the local endpoint for the socket.  
            // The DNS name of the computer  
            // running the listener is "host.contoso.com".  
            //IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            //IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 8081);

            // Create a TCP/IP socket.  
            Socket listener = new Socket(ipAddress.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);

            // Bind the socket to the local endpoint and listen for incoming connections.  
            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(100);

                while (listening)
                {
                    // Set the event to nonsignaled state.  
                    allDone.Reset();

                    // Start an asynchronous socket to listen for connections.  
                    Console.WriteLine("Waiting for a connection...");
                    listener.BeginAccept(
                        new AsyncCallback(AcceptCallback),
                        listener);

                    // Wait until a connection is made before continuing.  
                    allDone.WaitOne();
                }

            }
            catch (Exception e)
            {
                logChange.OnNext(new LogChange() { NewLog = new LogUnit(e.Message) });
            }

            Console.WriteLine("\nPress ENTER to continue...");
            Console.Read();

        }

        public static void AcceptCallback(IAsyncResult ar)
        {
            // Signal the main thread to continue.  
            allDone.Set();

            // Get the socket that handles the client request.  
            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);

            // Create the state object.  
            StateObject state = new StateObject();
            state.workSocket = handler;
            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                new AsyncCallback(ReadCallback), state);
        }

        public static void ReadCallback(IAsyncResult ar)
        {
            String content = String.Empty;

            // Retrieve the state object and the handler socket  
            // from the asynchronous state object.  
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;

            // Read data from the client socket.
            int bytesRead = handler.EndReceive(ar);

            if (bytesRead > 0)
            {
                // There  might be more data, so store the data received so far.  
                state.sb.Append(Encoding.ASCII.GetString(
                    state.buffer, 0, bytesRead));

                // Check for end-of-file tag. If it is not there, read
                // more data.  
                content = state.sb.ToString();
                //if (content.IndexOf("<EOF>") > -1)
                //{
                    // All the data has been read from the
                    // client. Display it on the console.  
                    //Console.WriteLine("Read {0} bytes from socket. \n Data : {1}", content.Length, content);
                    Subscriber sub;
                    BaseStation bs;
                    switch (content[0])
                    {
                        case '0':
                            bs = BaseStation.Deserialize(content.Substring(1));
                            Client.client = new TcpClient(bs.ip, Int32.Parse(bs.port));
                            Client.stream = Client.client.GetStream();
                            break;
                        case '1':
                            switch (content[1])
                            {
                                case '1':
                                    bs = BaseStation.Deserialize(content.Substring(2));
                                    bsChange.OnNext(new BaseStationChange() { AddRemove = "+", NewBS = bs });
                                    break;
                                case '2':
                                    bs = BaseStation.Deserialize(content.Substring(2));
                                    bsChange.OnNext(new BaseStationChange() { AddRemove = "-", NewBS = bs });
                                    break;
                                case '3':
                                    bs = BaseStation.Deserialize(content.Substring(2));
                                    bsChange.OnNext(new BaseStationChange() { AddRemove = "+-", NewBS = bs });
                                    break;
                            }
                            break;
                        case '2':
                            switch (content[1])
                            {
                                case '1':
                                    sub = Subscriber.Deserialize(content.Substring(2));
                                    sub.ParseClassmark();
                                    subChange.OnNext(new SubscribersChange() { AddRemove = "+", NewSub = sub });
                                    break;
                                case '2':
                                    sub = Subscriber.Deserialize(content.Substring(2));
                                    subChange.OnNext(new SubscribersChange() { AddRemove = "-", NewSub = sub });
                                    break;
                            }
                            break;
                        case '3':
                            switch (content[1])
                            {
                                case '1':
                                    TA ta = TA.Deserialize(content.Substring(2));
                                    taChange.OnNext(new TAChange() { NewTA = ta });
                                    break;
                                case '2':
                                    sub = Subscriber.Deserialize(content.Substring(2));
                                    geoChange.OnNext(new GeolocationChange() { NewGeo = SplitAssist.GetGeolocation(sub) });
                                    break;
                                case '3':
                                    sub = Subscriber.Deserialize(content.Substring(2));
                                    string res;
                                    using (var client = new WebClient())
                                    {
                                        res = client.DownloadString(@"http://192.168.70.132/cgi-bin/rrlpserver.cgi?query=decode&apdu=" + sub.assistData);
                                        //res = client.DownloadString(@"http://192.168.70.132/cgi-bin/rrlpserver.cgi?query=apdu&apdu=" + sub.assistData);
                                    }
                                    logChange.OnNext(new LogChange()
                                    {
                                        NewLog = new LogUnit(String.Format(
                                        "Получены измерения местоположения для абонента: IMSI = {0}, IMEI-SV = {1} - {2}", sub.imsi, sub.imeiSV, res))
                                    });
                                    break;
                            }
                            break;
                        case '4':
                            bs = BaseStation.Deserialize(content.Substring(1));
                            bsChange.OnNext(new BaseStationChange() { AddRemove = "-all", NewBS = bs });
                            break;
                        case '9':
                            switch (content[1])
                            {
                                case '0':
                                    break;
                                case '1':
                                    break;
                            }
                            break;
                    }
                    // Echo the data back to the client.  
                    Send(handler, content);
                //}
                //else
                //{
                //    // Not all data received. Get more.  
                //    handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                //    new AsyncCallback(ReadCallback), state);
                //}
            }
        }

        private static void Send(Socket handler, String data)
        {
            // Convert the string data to byte data using ASCII encoding.  
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            // Begin sending the data to the remote device.  
            handler.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), handler);
        }

        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                Socket handler = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.  
                int bytesSent = handler.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to client.", bytesSent);

                handler.Shutdown(SocketShutdown.Both);
                handler.Close();

            }
            catch (Exception e)
            {
                logChange.OnNext(new LogChange() { NewLog = new LogUnit(e.Message) });
            }
        }

        public static void StopListening()
        {
            listening = false;
            allDone.Set();
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
            Client.SendMessage(num.ToString() + sub.Serialize());
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
            GetGeolocation(6, "");
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
                cellids.Add(CellIDs.NewCellID);
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