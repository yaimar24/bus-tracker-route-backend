namespace BusTracker.Models
{
    public class BusCurrentPosition
    {
        public int BusId { get; set; }                    // PK y FK
        public Bus? Bus { get; set; }
        public double Speed { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double? SpeedKph { get; set; }
        public DateTime Timestamp { get; set; }           // UTC
        public string Source { get; set; } = "gps";       // gps / user
    }
}
