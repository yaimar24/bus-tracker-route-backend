using BusTracker.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("[controller]")]
public class BusesController : ControllerBase
{
    private readonly AppDbContext _db;

    public BusesController(AppDbContext db) { _db = db; }

    // GET /buses
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] int? routeId)
    {
        var query = _db.BusCurrentPositions.AsNoTracking().AsQueryable();

        if (routeId.HasValue)
        {
            query = from cp in _db.BusCurrentPositions
                    join a in _db.BusAssignments on cp.BusId equals a.BusId
                    where a.IsActive && a.BusRouteId == routeId.Value
                    select cp;
        }

        var results = await query
            .OrderByDescending(p => p.Timestamp)
            .Take(200)
            .ToListAsync();

        return Ok(results);
    }
}
