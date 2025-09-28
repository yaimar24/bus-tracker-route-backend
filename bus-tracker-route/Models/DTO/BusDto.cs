namespace bus_tracker_route.Models.DTO
{
    public class BusDto
    {
        public int Id { get; set; }
        public string PlateNumber { get; set; } = "";
        public string? Model { get; set; }
        public int Capacity { get; set; }
        public bool IsActive { get; set; }
    }

}
