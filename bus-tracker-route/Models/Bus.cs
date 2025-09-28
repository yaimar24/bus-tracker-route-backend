namespace BusTracker.Models
{
    public class Bus
    {
        public int Id { get; set; }                       // PK
        public string PlateNumber { get; set; } = "";     // matrícula única
        public string? Model { get; set; }
        public int? Capacity { get; set; }
        public bool IsActive { get; set; } = true;
        public int Year { get; set; } = DateTime.Now.Year;

        // FK
        public int CompanyId { get; set; }
        public Company? Company { get; set; }

        // Relaciones
        public ICollection<BusAssignment>? Assignments { get; set; }
        public BusCurrentPosition? CurrentPosition { get; set; }
        public ICollection<BusPositionHistory>? History { get; set; }
    }
}
