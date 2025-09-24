namespace BusTracker.Models
{
    public class BusPositionDto
    {
        public int BusId { get; set; }         
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double? SpeedKph { get; set; }
        public DateTime Timestamp { get; set; }
        public string? Source { get; set; }
    }
}
