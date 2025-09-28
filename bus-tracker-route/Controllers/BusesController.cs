using BusTracker.Data;
using BusTracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// DTO para evitar ciclos y controlar la respuesta
public class BusDto
{
    public int Id { get; set; }
    public string PlateNumber { get; set; } = "";
    public string? Model { get; set; }
    public int? Capacity { get; set; }
    public bool IsActive { get; set; }
    public int Year { get; set; }
    public int CompanyId { get; set; }
    public string CompanyName { get; set; } = "";
}

[ApiController]
[Route("api/[controller]")]
public class BusesController : ControllerBase
{
    private readonly AppDbContext _db;

    public BusesController(AppDbContext db)
    {
        _db = db;
    }

    // =======================
    // POSITIONS (tracking)
    // =======================

    // GET /api/buses?routeId=&busId=
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int? routeId, [FromQuery] int? busId)
    {
        var query = _db.BusCurrentPositions.AsNoTracking().AsQueryable();

        if (routeId.HasValue)
        {
            query = from cp in _db.BusCurrentPositions
                    join a in _db.BusAssignments on cp.BusId equals a.BusId
                    where a.IsActive && a.BusRouteId == routeId.Value
                    select cp;
        }

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

    // GET /api/buses/{busId}
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

    // GET /api/buses/{busId}/history
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

    // =======================
    // CRUD sobre tabla Buses
    // =======================

    // GET /api/buses/all
    [HttpGet("all")]
    public async Task<IActionResult> GetAllBuses()
    {
        var buses = await _db.Buses
            .Include(b => b.Company)
            .AsNoTracking()
            .Select(b => new BusDto
            {
                Id = b.Id,
                PlateNumber = b.PlateNumber,
                Model = b.Model,
                Capacity = b.Capacity,
                IsActive = b.IsActive,
                Year = b.Year,
                CompanyId = b.CompanyId,
                CompanyName = b.Company != null ? b.Company.Name : ""
            })
            .ToListAsync();

        return Ok(buses);
    }

    // GET /api/buses/all/{id}
    [HttpGet("all/{id:int}")]
    public async Task<IActionResult> GetBusById(int id)
    {
        var bus = await _db.Buses
            .Include(b => b.Company)
            .AsNoTracking()
            .Where(b => b.Id == id)
            .Select(b => new BusDto
            {
                Id = b.Id,
                PlateNumber = b.PlateNumber,
                Model = b.Model,
                Capacity = b.Capacity,
                IsActive = b.IsActive,
                Year = b.Year,
                CompanyId = b.CompanyId,
                CompanyName = b.Company != null ? b.Company.Name : ""
            })
            .FirstOrDefaultAsync();

        if (bus == null)
            return NotFound(new { Message = $"No existe el bus con Id {id}" });

        return Ok(bus);
    }

    // POST /api/buses/all
    [HttpPost("all")]
    public async Task<IActionResult> CreateBus([FromBody] Bus bus)
    {
        if (bus == null)
            return BadRequest(new { Message = "El bus es obligatorio." });

        if (string.IsNullOrWhiteSpace(bus.PlateNumber))
            return BadRequest(new { Message = "La matrícula es obligatoria." });

        var companyExists = await _db.Companies.AnyAsync(c => c.Id == bus.CompanyId);
        if (!companyExists)
            return BadRequest(new { Message = $"No existe la compañía con Id {bus.CompanyId}" });

        var plateExists = await _db.Buses.AnyAsync(b => b.PlateNumber == bus.PlateNumber);
        if (plateExists)
            return Conflict(new { Message = "Ya existe un bus con esa matrícula." });

        bus.IsActive = true;

        _db.Buses.Add(bus);
        await _db.SaveChangesAsync();

        // devolver en formato DTO
        var result = new BusDto
        {
            Id = bus.Id,
            PlateNumber = bus.PlateNumber,
            Model = bus.Model,
            Capacity = bus.Capacity,
            IsActive = bus.IsActive,
            Year = bus.Year,
            CompanyId = bus.CompanyId,
            CompanyName = (await _db.Companies.FindAsync(bus.CompanyId))?.Name ?? ""
        };

        return CreatedAtAction(nameof(GetBusById), new { id = bus.Id }, result);
    }

    // PUT /api/buses/all/{id}
    [HttpPut("all/{id:int}")]
    public async Task<IActionResult> UpdateBus(int id, [FromBody] Bus updatedBus)
    {
        var bus = await _db.Buses.FindAsync(id);
        if (bus == null)
            return NotFound(new { Message = $"No existe el bus con Id {id}" });

        bus.PlateNumber = updatedBus.PlateNumber;
        bus.Model = updatedBus.Model;
        bus.Capacity = updatedBus.Capacity;
        bus.CompanyId = updatedBus.CompanyId;
        bus.Year = updatedBus.Year;
        bus.IsActive = updatedBus.IsActive;

        await _db.SaveChangesAsync();

        var result = new BusDto
        {
            Id = bus.Id,
            PlateNumber = bus.PlateNumber,
            Model = bus.Model,
            Capacity = bus.Capacity,
            IsActive = bus.IsActive,
            Year = bus.Year,
            CompanyId = bus.CompanyId,
            CompanyName = (await _db.Companies.FindAsync(bus.CompanyId))?.Name ?? ""
        };

        return Ok(result);
    }
}
