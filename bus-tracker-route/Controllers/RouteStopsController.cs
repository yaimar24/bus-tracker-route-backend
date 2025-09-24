using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BusTracker.Data;
using BusTracker.Models;

namespace BusTracker.Controllers
{
    [ApiController]
    [Route("[controller]")]
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
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RouteStop stop)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            _db.RouteStops.Add(stop);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = stop.Id }, stop);
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
