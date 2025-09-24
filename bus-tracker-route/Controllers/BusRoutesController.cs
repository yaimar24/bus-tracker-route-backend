using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BusTracker.Data;
using BusTracker.Models;

namespace BusTracker.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BusRoutesController : ControllerBase
    {
        private readonly AppDbContext _db;

        public BusRoutesController(AppDbContext db)
        {
            _db = db;
        }

        // GET /busroutes
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var routes = await _db.BusRoutes
                .Include(r => r.Stops)
                .AsNoTracking()
                .ToListAsync();

            var dto = routes.Select(r => new BusRouteDto
            {
                Id = r.Id,
                Name = r.Name,
                Description = r.Description,
                Origin = r.Origin,
                Destination = r.Destination,
                IsActive = r.IsActive,
                Stops = r.Stops?
                    .OrderBy(s => s.Order)
                    .Select(s => new RouteStopDto
                    {
                        Id = s.Id,
                        Name = s.Name,
                        Order = s.Order
                    }).ToList() ?? new List<RouteStopDto>()
            });

            return Ok(dto);
        }

        // GET /busroutes/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var route = await _db.BusRoutes
                .Include(r => r.Stops)
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == id);

            if (route == null) return NotFound();

            var dto = new BusRouteDto
            {
                Id = route.Id,
                Name = route.Name,
                Description = route.Description,
                Origin = route.Origin,
                Destination = route.Destination,
                IsActive = route.IsActive,
                Stops = route.Stops?
                    .OrderBy(s => s.Order)
                    .Select(s => new RouteStopDto
                    {
                        Id = s.Id,
                        Name = s.Name,
                        Order = s.Order
                    }).ToList() ?? new List<RouteStopDto>()
            };

            return Ok(dto);
        }

        // POST /busroutes
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BusRoute route)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            _db.BusRoutes.Add(route);
            await _db.SaveChangesAsync();

            var dto = new BusRouteDto
            {
                Id = route.Id,
                Name = route.Name,
                Description = route.Description,
                Origin = route.Origin,
                Destination = route.Destination,
                IsActive = route.IsActive,
                Stops = new List<RouteStopDto>() // Inicialmente vacío
            };

            return CreatedAtAction(nameof(GetById), new { id = route.Id }, dto);
        }

        // PUT /busroutes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] BusRoute updated)
        {
            var route = await _db.BusRoutes.FindAsync(id);
            if (route == null) return NotFound();

            route.Name = updated.Name;
            route.Description = updated.Description;
            route.Origin = updated.Origin;
            route.Destination = updated.Destination;
            route.IsActive = updated.IsActive;

            await _db.SaveChangesAsync();
            return NoContent();
        }

        // DELETE /busroutes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var route = await _db.BusRoutes.FindAsync(id);
            if (route == null) return NotFound();

            _db.BusRoutes.Remove(route);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }

    // DTOs
    public class BusRouteDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string? Description { get; set; }
        public string? Origin { get; set; }
        public string? Destination { get; set; }
        public bool IsActive { get; set; }
        public List<RouteStopDto> Stops { get; set; } = new();
    }

    public class RouteStopDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public int Order { get; set; }
    }
}
