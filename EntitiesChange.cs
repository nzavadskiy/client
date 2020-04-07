namespace client
{
    public class ClientChange
    {
        public string AddRemove { get; set; }
        public Client NewClient { get; set; }
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
}