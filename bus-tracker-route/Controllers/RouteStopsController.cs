using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BusTracker.Data;
using BusTracker.Models;

namespace BusTracker.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RouteStopsController : ControllerBase
    {
        private readonly AppDbContext _db;

        public RouteStopsController(AppDbContext db)
        {
            _db = db;
        }

        // GET /routestops?routeId=5
        // Lista todas las paradas, con filtro opcional por ruta
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int? routeId)
        {
            var query = _db.RouteStops
                .Include(rs => rs.BusRoute)
                .AsQueryable();

            if (routeId.HasValue)
                query = query.Where(rs => rs.BusRouteId == routeId.Value);

            var stops = await query
                .OrderBy(rs => rs.Order)
                .AsNoTracking()
                .ToListAsync();

            return Ok(stops);
        }

        // GET /routestops/10
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var stop = await _db.RouteStops
                .Include(rs => rs.BusRoute)
                .AsNoTracking()
                .FirstOrDefaultAsync(rs => rs.Id == id);

            if (stop == null) return NotFound();

            return Ok(stop);
        }

        // POST /routestops
        // Registrar una nueva parada
        // POST /busroutes
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
                Stops = dto.Stops.Select(s => new RouteStop
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
            dto.Stops = route.Stops.Select(s => new RouteStopDto
            {
                Id = s.Id,
                Name = s.Name,
                Latitude = s.Latitude,
                Longitude = s.Longitude,
                Order = s.Order
            }).ToList();

            return CreatedAtAction(nameof(GetById), new { id = route.Id }, dto);
        }


        // PUT /routestops/10
        // Actualizar datos de una parada
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] RouteStop updated)
        {
            var stop = await _db.RouteStops.FindAsync(id);
            if (stop == null) return NotFound();

            stop.Name = updated.Name;
            stop.Latitude = updated.Latitude;
            stop.Longitude = updated.Longitude;
            stop.Order = updated.Order;
            stop.BusRouteId = updated.BusRouteId;

            await _db.SaveChangesAsync();
            return NoContent();
        }

        // DELETE /routestops/10
        // Eliminar una parada
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var stop = await _db.RouteStops.FindAsync(id);
            if (stop == null) return NotFound();

            _db.RouteStops.Remove(stop);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
