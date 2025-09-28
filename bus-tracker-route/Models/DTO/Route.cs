namespace bus_tracker_route.Models.DTO
{
    public class RouteDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<RouteStopDto> Stops { get; set; }
    }
}
