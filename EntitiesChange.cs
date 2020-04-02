namespace client
{
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
}