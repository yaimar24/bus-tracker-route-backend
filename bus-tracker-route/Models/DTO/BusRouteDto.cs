public class BusRouteDto
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string? Description { get; set; }

    public List<RouteStopDto> Stops { get; set; } = new();
}

public class RouteStopDto
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
}
