

using System.Text.Json.Serialization;

namespace BusTracker.Models
{

    public class BusRoute
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string? Description { get; set; }
        public string? Origin { get; set; }
        public string? Destination { get; set; }
        public bool IsActive { get; set; } = true;
        [JsonIgnore]
        public ICollection<RouteStop>? Stops { get; set; }
        public ICollection<BusAssignment>? Assignments { get; set; }
    }

}
