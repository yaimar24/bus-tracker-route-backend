namespace BusTracker.Models
{
    public class BusPosition
    {
        public int Id { get; set; }                // PK
        public string BusId { get; set; } = "";   // identificador del bus
        public string Route { get; set; } = "";   // ruta (R10, etc)
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double? SpeedKph { get; set; }
        public DateTime Timestamp { get; set; }   // UTC
        public string Source { get; set; } = "gps";// "gps" o "user"
    }
}
