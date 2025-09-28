using BusTracker.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("[controller]")]
public class BusesController : ControllerBase
{
    private readonly AppDbContext _db;

    public BusesController(AppDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// GET /Buses
    /// Devuelve hasta 200 posiciones de buses.
    /// Puede filtrarse por routeId y/o busId con query string.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int? routeId, [FromQuery] int? busId)
    {
        var query = _db.BusCurrentPositions.AsNoTracking().AsQueryable();

        // Filtrar por ruta
        if (routeId.HasValue)
        {
            query = from cp in _db.BusCurrentPositions
                    join a in _db.BusAssignments on cp.BusId equals a.BusId
                    where a.IsActive && a.BusRouteId == routeId.Value
                    select cp;
        }

        // Filtrar por bus
        if (busId.HasValue)
        {
            query = query.Where(cp => cp.BusId == busId.Value);
        }

        var results = await query
            .OrderByDescending(p => p.Timestamp)
            .Take(200)
            .ToListAsync();

        return Ok(results);
    }

    /// <summary>
    /// GET /Buses/{busId}
    /// Devuelve la última posición de un bus específico.
    /// </summary>
    [HttpGet("{busId:int}")]
    public async Task<IActionResult> GetById(int busId)
    {
        var position = await _db.BusCurrentPositions
            .AsNoTracking()
            .Where(cp => cp.BusId == busId)
            .OrderByDescending(cp => cp.Timestamp)
            .FirstOrDefaultAsync();

        if (position == null)
            return NotFound(new { Message = $"No hay posiciones para el bus {busId}" });

        return Ok(position);
    }

    /// <summary>
    /// GET /Buses/{busId}/history
    /// Devuelve hasta 500 posiciones históricas de un bus específico (más recientes primero).
    /// </summary>
    [HttpGet("{busId:int}/history")]
    public async Task<IActionResult> GetHistory(int busId)
    {
        var history = await _db.BusCurrentPositions
            .AsNoTracking()
            .Where(cp => cp.BusId == busId)
            .OrderByDescending(cp => cp.Timestamp)
            .Take(500)
            .ToListAsync();

        if (history == null || history.Count == 0)
            return NotFound(new { Message = $"No hay historial para el bus {busId}" });

        return Ok(history);
    }
}
