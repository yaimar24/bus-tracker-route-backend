namespace BusTracker.Models
{
    public class Company
    {
        public int Id { get; set; }                       // PK
        public string Name { get; set; } = string.Empty;
        public string? ContactInfo { get; set; }

        // Relaciones
        public ICollection<Bus>? Buses { get; set; }
        public ICollection<Driver>? Drivers { get; set; }
    }
}
