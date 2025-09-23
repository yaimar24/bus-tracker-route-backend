using Microsoft.AspNetCore.Mvc;
using BusTracker.Data;
using Microsoft.EntityFrameworkCore;

namespace BusTracker.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BusesController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly TimeSpan MaxAge = TimeSpan.FromMinutes(15);

        public BusesController(AppDbContext db) { _db = db; }

        // GET /buses?route=R10
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string? route, [FromQuery] double? lat, [FromQuery] double? lng, [FromQuery] double? radiusMeters)
        {
            var cutoff = DateTime.Now - MaxAge;
            var query = _db.BusPositions.AsNoTracking().Where(p => p.Timestamp >= cutoff);

            if (!string.IsNullOrEmpty(route))
                query = query.Where(p => p.Route == route);

            // Simple proximity filter (Haversine would be better)
            if (lat.HasValue && lng.HasValue && radiusMeters.HasValue)
            {
                double r = radiusMeters.Value;
                // For simplicity: filter by bounding box approx
                double latDelta = r / 111000.0;
                double lngDelta = Math.Abs(r / (111000.0 * Math.Cos(lat.Value * Math.PI / 180)));
                query = query.Where(p =>
                    p.Latitude >= lat - latDelta && p.Latitude <= lat + latDelta &&
                    p.Longitude >= lng - lngDelta && p.Longitude <= lng + lngDelta);
            }




            var results = await _db.BusPositions
                .AsNoTracking()
                .Where(p => p.Timestamp >= cutoff &&
                            (string.IsNullOrEmpty(route) || p.Route == route))
                .ToListAsync(); 

            var latest = results
                .GroupBy(p => p.BusId)
                .Select(g => g.OrderByDescending(p => p.Timestamp).First())
                .OrderByDescending(p => p.Timestamp)
                .Take(200)
                .Select(p => new {
                    busId = p.BusId,
                    route = p.Route,
                    lat = p.Latitude,
                    lng = p.Longitude,
                    speed = p.SpeedKph,
                    timestamp = p.Timestamp,
                    source = p.Source
                })
                .ToList();

            return Ok(latest);


        }
    }
}
