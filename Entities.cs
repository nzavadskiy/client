using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.IO;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Net.Sockets;
using System.Globalization;

namespace client
{
    [DataContract]
    public class BaseStation : INotifyPropertyChanged
    {
        public string _name;
        public string _mcc;
        public string _mnc;
        public string _cellid;
        public string _lac;
        public string _lat;
        public string _lon;
        public string _alt;
        public string _accuracy;
        public string _antenna;
        public string _ip;
        public string _port;
        public event PropertyChangedEventHandler PropertyChanged;    
        public void OnPropertyChanged(string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
        [DataMember]
        public string name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged("name");
                }
            }
        }
        [DataMember]
        public string mcc
        {
            get { return _mcc; }
            set
            {
                if (_mcc != value)
                {
                    _mcc = value;
                    OnPropertyChanged("mcc");
                }
            }
        }
        [DataMember]
        public string mnc
        {
            get { return _mnc; }
            set
            {
                if (_mnc != value)
                {
                    _mnc = value;
                    OnPropertyChanged("mnc");
                }
            }
        }
        [DataMember]
        public string cellid
        {
            get { return _cellid; }
            set
            {
                if (_cellid != value)
                {
                    _cellid = value;
                    OnPropertyChanged("cellid");
                }
            }
        }
        [DataMember]
        public string lac
        {
            get { return _lac; }
            set
            {
                if (_lac != value)
                {
                    _lac = value;
                    OnPropertyChanged("lac");
                }
            }
        }
        [DataMember]
        public string lat
        {
            get { return _lat; }
            set
            {
                if (_lat != value)
                {
                    _lat = value;
                    OnPropertyChanged("lat");
                }
            }
        }
        [DataMember]
        public string lon
        {
            get { return _lon; }
            set
            {
                if (_lon != value)
                {
                    _lon = value;
                    OnPropertyChanged("lon");
                }
            }
        }
        [DataMember]
        public string alt
        {
            get { return _alt; }
            set
            {
                if (_alt != value)
                {
                    _alt = value;
                    OnPropertyChanged("alt");
                }
            }
        }
        [DataMember]
        public string accuracy
        {
            get { return _accuracy; }
            set
            {
                if (_accuracy != value)
                {
                    _accuracy = value;
                    OnPropertyChanged("accuracy");
                }
            }
        }
        [DataMember]
        public string antenna
        {
            get { return _antenna; }
            set
            {
                if (_antenna != value)
                {
                    _antenna = value;
                    OnPropertyChanged("antenna");
                }
            }
        }
        [DataMember]
        public string ip
        {
            get { return _ip; }
            set
            {
                if (_ip != value)
                {
                    _ip = value;
                    OnPropertyChanged("ip");
                }
            }
        }
        [DataMember]
        public string port
        {
            get { return _port; }
            set
            {
                if (_port != value)
                {
                    _port = value;
                    OnPropertyChanged("port");
                }
            }
        }
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
        public BaseStation(string name)
        {
            this.name = name;
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
            //string bsCoordResStr;
            //using (var client = new WebClient())
            //{
            //    bsCoordResStr = client.DownloadString(String.Format("https://api.mylnikov.org/geolocation/cell?v=1.1&data=open&mcc={0}&mnc={1}&lac={2}&cellid={3}", mcc, mnc, lac, cellid));
            //}
            //dynamic bsCoordRes = JsonConvert.DeserializeObject(bsCoordResStr);
            //if (bsCoordRes.ContainsKey("data"))
            //{
            //    if (bsCoordRes.data.ContainsKey("lat"))
            //    {
            //        lat = bsCoordRes.data.lat;
            //    }
            //    if (bsCoordRes.data.ContainsKey("lon"))
            //    {
            //        lon = bsCoordRes.data.lon;
            //    }
            //}
            var rand = new Random();            
            lat = (rand.NextDouble() + 55).ToString(CultureInfo.InvariantCulture);
            lon = (rand.NextDouble() + 37).ToString(CultureInfo.InvariantCulture);
            antenna = "-" + ((rand.NextDouble() * 100 + 60) % 100).ToString(CultureInfo.InvariantCulture);
        }
        public void GetFieldsFromTAMsg(dynamic neighbourBS)
        {
            if (neighbourBS.ContainsKey("mcc"))
                mcc = neighbourBS.mcc;
            if (neighbourBS.ContainsKey("mnc"))
                mnc = neighbourBS.mnc;
            if (neighbourBS.ContainsKey("lac"))
                lac = neighbourBS.lac;
            if (neighbourBS.ContainsKey("cellid"))
                cellid = neighbourBS.cellid;
            if (neighbourBS.ContainsKey("antenna"))
                antenna = neighbourBS.antenna;
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
        public string lat { get; set; }
        [DataMember]
        public string lon { get; set; }
        [DataMember]
        public string date { get; set; }
        [DataMember]
        public string servingBSName { get; set; }
        [DataMember]
        public string servingBSTA { get; set; }
        [DataMember]
        public string servingBSLev { get; set; }
        [DataMember]
        public const int neighboursCount = 6;
        [DataMember]
        public BaseStation[] neighbours;
        public TA()
        {
            neighbours = new BaseStation[neighboursCount];
            for(int i = 0; i < neighboursCount; i++)
            {
                neighbours[i] = new BaseStation();
            }
        }
        public void GetFieldsFromTAMsg(dynamic neighbours)
        {
            for (int i = 0; i < neighboursCount; i++)
            {
                this.neighbours[i].GetFieldsFromTAMsg(neighbours[i]);
            }
        }
        public void CalculateCoords()
        {
            double[] weights = new double[neighboursCount];
            double sumWeights = 0;
            double latSum = 0;
            double lonSum = 0;
            for (int i = 0; i < neighboursCount; i++)
            {
                if (neighbours[i].mcc != "")
                {
                    neighbours[i].GetBSCoord();
                    double weight = Math.Pow(10, double.Parse(neighbours[i].antenna, CultureInfo.InvariantCulture) / 20.0);
                    sumWeights += weight;
                    latSum += weight * double.Parse(neighbours[i].lat, CultureInfo.InvariantCulture);
                    lonSum += weight * double.Parse(neighbours[i].lon, CultureInfo.InvariantCulture);
                }
            }
            lat = (latSum / sumWeights).ToString(CultureInfo.InvariantCulture);
            lon = (lonSum / sumWeights).ToString(CultureInfo.InvariantCulture);
        }
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
        public bool GetDistFromTA()
        {
            int intDist;
            if(int.TryParse(dist, out intDist))
            {
                intDist *= 554;
                dist = String.Format("{0} - {1} метров", intDist.ToString(), (intDist + 554).ToString());
                return true;
            }
            else
            {
                return false;
            }
        }

    }
    public class Client
    {
        public TcpClient client;
        public IPAddress ip { get; set; }
        public NetworkStream stream { get; set; }
        public string ipString;
        public string portString;
        public void Start() { }

        public Client() { }
        public Client(string ip, string port)
        {
            ipString = ip;
            portString = port;
            client = new TcpClient(ip, Int32.Parse(port));
            stream = client.GetStream();
        }
        public int SendMessage(string message)
        {
            Byte[] data = Encoding.ASCII.GetBytes(message);
            stream.Write(data, 0, data.Length);
            return 0;
        }
        public string GetMessage()
        {
            byte[] data = new Byte[1024];
            int bytes = stream.Read(data, 0, data.Length);
            return Encoding.ASCII.GetString(data, 0, bytes);
        }
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            Client c = (Client)obj;
            return ipString == c.ipString && portString == c.portString;
        }
        public void CloseClient()
        {
            stream.Close();
            client.Close();
        }
        public override int GetHashCode()
        {
            var hashCode = -449251652;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ipString);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(portString);
            return hashCode;
        }
    }
    public class QueryParameters
    {
        public int responseTime;
        public int accuracy;
        public int numOfAssists;
        public double latitude;
        public double longtitude;
        public int altitude;
        public int refreshInterval;
        public double timeRefreshAmanac;
        public double timeRefreshEphem;
        private const string almanacURL = "687474703a2f2f7777772e6e617663656e2e757363672e676f762f3f706167654e616d653d63757272656e74416c6d616e616326666f726d61743d79756d61";
        private const string ephemerisURL = "6674703a2f2f6674702e7472696d626c652e636f6d2f7075622f6570682f437572526e784e2e6e6176";
        public QueryParameters()
        {
            timeRefreshAmanac = 24;
            timeRefreshEphem = 0.1;
        }
        public QueryParameters(int rt, int acc, int num, double lat, double lon, int alt, int refrInt)
        {
            responseTime = rt;
            accuracy = acc;
            numOfAssists = num;
            latitude = lat;
            longtitude = lon;
            altitude = alt;
            timeRefreshAmanac = 24;
            timeRefreshEphem = 0.1;
            refreshInterval = refrInt;
        }
        public void GetParamsFromFile(string fileName)
        {
            string content = File.ReadAllText(fileName);
            //string content = File.ReadAllText("..\\..\\configs\\renewAddInfo.json");
            dynamic paramsConf = JsonConvert.DeserializeObject(content);
            if (paramsConf.ContainsKey("responseTime"))
            {
                responseTime = paramsConf.responseTime;
            }
            if (paramsConf.ContainsKey("accuracy"))
            {
                accuracy = paramsConf.accuracy;
            }
            if (paramsConf.ContainsKey("numOfAssists"))
            {
                numOfAssists = paramsConf.numOfAssists;
            }
            if (paramsConf.ContainsKey("longtitude"))
            {
                longtitude = paramsConf.longtitude;
            }
            if (paramsConf.ContainsKey("altitude"))
            {
                altitude = paramsConf.altitude;
            }
            if (paramsConf.ContainsKey("refreshInterval"))
            {
                refreshInterval = paramsConf.refreshInterval;
            }
            if (paramsConf.ContainsKey("latitude"))
            {
                latitude = paramsConf.latitude;
            }
        }
        public string GetQuery()
        {
            return String.Format(@"http://192.168.70.132/cgi-bin/rrlpserver.cgi?GSM.RRLP.ACCURACY={1}&GSM.RRLP.RESPONSETIME={0}&GSM.RRLP.ALMANAC.URL={8}&GSM.RRLP.EPHEMERIS.URL={9}&GSM.RRLP.ALMANAC.REFRESH.TIME={6}&GSM.RRLP.EPHEMERIS.REFRESH.TIME={7}&GSM.RRLP.SEED.LATITUDE={3}.0&GSM.RRLP.SEED.LONGITUDE={4}.0&GSM.RRLP.SEED.ALTITUDE={5}&GSM.RRLP.ALMANAC.ASSIST.PRESENT=0&GSM.RRLP.EPHEMERIS.ASSIST.COUNT={2}&query=assist",
                responseTime, accuracy, numOfAssists, latitude, longtitude, altitude, timeRefreshAmanac, timeRefreshEphem, almanacURL, ephemerisURL);
        }
    }
}