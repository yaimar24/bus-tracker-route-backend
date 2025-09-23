using Microsoft.AspNetCore.Mvc;
using BusTracker.Data;
using BusTracker.Models;
using Microsoft.AspNetCore.SignalR;
using BusTracker.Hubs;

namespace BusTracker.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GpsController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IHubContext<PositionsHub> _hub;

        public GpsController(AppDbContext db, IHubContext<PositionsHub> hub)
        {
            _db = db;
            _hub = hub;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] BusPositionDto dto)
        {
            if (dto == null) return BadRequest();

            var pos = new BusPosition
            {
                BusId = dto.BusId ?? Guid.NewGuid().ToString(),
                Route = dto.Route ?? "",
                Latitude = dto.Latitude,
                Longitude = dto.Longitude,
                SpeedKph = dto.SpeedKph,
                Timestamp = dto.Timestamp == default ? DateTime.UtcNow : dto.Timestamp.ToUniversalTime(),
                Source = dto.Source ?? "gps"
            };

            _db.BusPositions.Add(pos);
            await _db.SaveChangesAsync();

            // Emitir por SignalR a todos o al grupo de la ruta
            if (!string.IsNullOrEmpty(pos.Route))
            {
                await _hub.Clients.Group(pos.Route).SendAsync("ReceivePosition", new
                {
                    busId = pos.BusId,
                    route = pos.Route,
                    lat = pos.Latitude,
                    lng = pos.Longitude,
                    timestamp = pos.Timestamp,
                    source = pos.Source
                });
            }
            else
            {
                await _hub.Clients.All.SendAsync("ReceivePosition", new
                {
                    busId = pos.BusId,
                    route = pos.Route,
                    lat = pos.Latitude,
                    lng = pos.Longitude,
                    timestamp = pos.Timestamp,
                    source = pos.Source
                });
            }

            return Ok(new { success = true, id = pos.Id });
        }
    }

    public class BusPositionDto
    {
        public string? BusId { get; set; }
        public string? Route { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double? SpeedKph { get; set; }
        public DateTime Timestamp { get; set; }
        public string? Source { get; set; }
    }
}
