namespace BusTracker.Models
{
    public class Driver
    {
        public int Id { get; set; }                       // PK
        public string FullName { get; set; } = "";
        public string LicenseNumber { get; set; } = "";
        public string? Phone { get; set; }
        public string? Name { get; set; }
        public bool IsActive { get; set; } = true;
        
        // FK
        public int CompanyId { get; set; }
        public Company? Company { get; set; }

        // Relaciones
        public ICollection<BusAssignment>? Assignments { get; set; }
    }
}
