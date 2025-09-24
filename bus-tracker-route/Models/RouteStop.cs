namespace BusTracker.Models
{
    public class RouteStop
    {
        public int Id { get; set; }                       // PK
        public string Name { get; set; } = "";
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int Order { get; set; }                    // posición en la ruta

        // FK
        public int BusRouteId { get; set; }
        public BusRoute? BusRoute { get; set; }
    }
}
