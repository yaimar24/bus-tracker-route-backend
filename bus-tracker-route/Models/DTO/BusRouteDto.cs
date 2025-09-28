using BusTracker.Controllers;
using bus_tracker_route.Models.DTO;

public class BusRouteDto
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string? Description { get; set; }
    public string? Origin { get; set; }
    public string? Destination { get; set; }
    public bool IsActive { get; set; }

    public int CompanyId { get; set; }
    public List<RouteStopDto> Stops { get; set; } = new();
}


public class RouteStopDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public int Order { get; set; }
}

public class BusWithRouteDto
{
    public int Id { get; set; }
    public string PlateNumber { get; set; }
    public string Model { get; set; }
    public int Capacity { get; set; }
    public bool IsActive { get; set; }
    public RouteDto Route { get; set; }
}