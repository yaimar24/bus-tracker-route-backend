namespace BusTracker.Models
{
    public class BusPositionHistory
    {
        public long Id { get; set; }                      // PK
        public int BusId { get; set; }
        public Bus? Bus { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double? SpeedKph { get; set; }
        public DateTime Timestamp { get; set; }
        public double Speed { get; set; }   // UTC
        public string Source { get; set; } = "gps";

    }
}
