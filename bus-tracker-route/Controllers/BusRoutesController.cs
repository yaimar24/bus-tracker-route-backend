using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BusTracker.Data;
using BusTracker.Models;
using bus_tracker_route.Models.DTO;

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
        [HttpGet("{routeId}/buses")]
        public async Task<IActionResult> GetBusesByRoute(int routeId)
        {
            // Verificar que la ruta exista
            var route = await _db.BusRoutes
                .Include(r => r.Stops.OrderBy(s => s.Order)) // incluimos las paradas en orden
                .FirstOrDefaultAsync(r => r.Id == routeId);

            if (route == null)
                return NotFound($"Ruta con id {routeId} no encontrada.");

            // Buscar buses asignados a la ruta
            var buses = await _db.BusAssignments
                .Where(a => a.BusRouteId == routeId)
                .Include(a => a.Bus) // cargamos la relación con Bus
                .Select(a => new BusWithRouteDto
                {
                    Id = a.Bus.Id,
                    PlateNumber = a.Bus.PlateNumber,
                    Model = a.Bus.Model,
                    Capacity = a.Bus.Capacity ?? 0,
                    IsActive = a.Bus.IsActive,
                    Route = new RouteDto
                    {
                        Id = route.Id,
                        Name = route.Name,
                        Stops = route.Stops.Select(s => new RouteStopDto
                        {
                            Id = s.Id,
                            Name = s.Name,
                            Latitude = s.Latitude,
                            Longitude = s.Longitude,
                            Order = s.Order
                        }).ToList()
                    }
                })
                .Distinct()
                .ToListAsync();

            return Ok(buses);
        }

    }


 


}
