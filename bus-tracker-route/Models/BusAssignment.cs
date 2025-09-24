namespace BusTracker.Models
{
    public class BusAssignment
    {
        public int Id { get; set; }                       // PK
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public bool IsActive { get; set; } = true;

        // FKs
        public int BusId { get; set; }
        public Bus? Bus { get; set; }

        public int DriverId { get; set; }
        public Driver? Driver { get; set; }

        public int BusRouteId { get; set; }            
        public BusRoute? BusRoute { get; set; }
    }
}
