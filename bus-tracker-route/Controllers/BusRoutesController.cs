using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BusTracker.Data;
using BusTracker.Models;
using bus_tracker_route.Models.DTO;

namespace BusTracker.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
                .Include(r => r.Company)
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
                CompanyId = r.CompanyId,
                Stops = r.Stops?
                    .OrderBy(s => s.Order)
                    .Select(s => new RouteStopDto
                    {
                        Id = s.Id,
                        Name = s.Name,
                        Latitude = s.Latitude,
                        Longitude = s.Longitude,
                        Order = s.Order
                    }).ToList() ?? new List<RouteStopDto>()
            });

            return Ok(dto);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var route = await _db.BusRoutes
                .Include(r => r.Stops)
                .Include(r => r.Company)
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
                CompanyId = route.CompanyId,
                Stops = route.Stops?
                    .OrderBy(s => s.Order)
                    .Select(s => new RouteStopDto
                    {
                        Id = s.Id,
                        Name = s.Name,
                        Latitude = s.Latitude,
                        Longitude = s.Longitude,
                        Order = s.Order
                    }).ToList() ?? new List<RouteStopDto>()
            };

            return Ok(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BusRouteDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var company = await _db.Companies.FindAsync(dto.CompanyId);
            if (company == null)
                return BadRequest($"No existe la compañía con id {dto.CompanyId}");

            var route = new BusRoute
            {
                Name = dto.Name,
                Description = dto.Description,
                Origin = dto.Origin,
                Destination = dto.Destination,
                IsActive = dto.IsActive,
                CompanyId = dto.CompanyId,
                Stops = dto.Stops?.Select(s => new RouteStop
                {
                    Name = s.Name,
                    Latitude = s.Latitude,
                    Longitude = s.Longitude,
                    Order = s.Order
                }).ToList()
            };

            _db.BusRoutes.Add(route);
            await _db.SaveChangesAsync();

            dto.Id = route.Id;
            dto.Stops = route.Stops?.Select(s => new RouteStopDto
            {
                Id = s.Id,
                Name = s.Name,
                Latitude = s.Latitude,
                Longitude = s.Longitude,
                Order = s.Order
            }).ToList();

            return CreatedAtAction(nameof(GetById), new { id = route.Id }, dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] BusRouteDto dto)
        {
            var route = await _db.BusRoutes
                .Include(r => r.Stops)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (route == null) return NotFound();

            var company = await _db.Companies.FindAsync(dto.CompanyId);
            if (company == null)
                return BadRequest($"No existe la compañía con id {dto.CompanyId}");

            // Actualizar ruta
            route.Name = dto.Name;
            route.Description = dto.Description;
            route.Origin = dto.Origin;
            route.Destination = dto.Destination;
            route.IsActive = dto.IsActive;
            route.CompanyId = dto.CompanyId;

            // Eliminar paradas que ya no existen en el DTO
            var stopsToRemove = route.Stops
                .Where(s => !dto.Stops.Any(ds => ds.Order == s.Order))
                .ToList();
            foreach (var stop in stopsToRemove)
                _db.RouteStops.Remove(stop);

            // Actualizar existentes o agregar nuevas
            foreach (var stopDto in dto.Stops)
            {
                var existingStop = route.Stops.FirstOrDefault(s => s.Order == stopDto.Order);
                if (existingStop != null)
                {
                    existingStop.Name = stopDto.Name;
                    existingStop.Latitude = stopDto.Latitude;
                    existingStop.Longitude = stopDto.Longitude;
                }
                else
                {
                    route.Stops.Add(new RouteStop
                    {
                        Name = stopDto.Name,
                        Latitude = stopDto.Latitude,
                        Longitude = stopDto.Longitude,
                        Order = stopDto.Order
                    });
                }
            }

            await _db.SaveChangesAsync();
            return NoContent();
        }

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
            var route = await _db.BusRoutes
                .Include(r => r.Stops.OrderBy(s => s.Order))
                .FirstOrDefaultAsync(r => r.Id == routeId);

            if (route == null)
                return NotFound($"Ruta con id {routeId} no encontrada.");

            var buses = await _db.BusAssignments
                .Where(a => a.BusRouteId == routeId)
                .Include(a => a.Bus)
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
