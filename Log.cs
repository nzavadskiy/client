using System;

namespace client
{
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
}