using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.IO;
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
}