namespace bus_tracker_route.Models.DTO
{
    public class BusAssignmentDto
    {
        public int Id { get; set; }
        public int BusId { get; set; }
        public int BusRouteId { get; set; }
        public int? DriverId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
    }

}
