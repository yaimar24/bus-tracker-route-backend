using BusTracker.Data;
using BusTracker.Hubs;
using BusTracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

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

        var now = dto.Timestamp == default ? DateTime.UtcNow : dto.Timestamp.ToUniversalTime();

        // Actualizar posición actual
        var current = await _db.BusCurrentPositions.FindAsync(dto.BusId);
        if (current == null)
        {
            current = new BusCurrentPosition
            {
                BusId = dto.BusId,
                Latitude = dto.Latitude,
                Longitude = dto.Longitude,
                Speed = dto.SpeedKph ?? 0,
                Timestamp = now
            };
            _db.BusCurrentPositions.Add(current);
        }
        else
        {
            current.Latitude = dto.Latitude;
            current.Longitude = dto.Longitude;
            current.Speed = dto.SpeedKph ?? 0;
            current.Timestamp = now;
        }

        // Guardar histórico
        var history = new BusPositionHistory
        {
            BusId = dto.BusId,
            Latitude = dto.Latitude,
            Longitude = dto.Longitude,
            Speed = dto.SpeedKph ?? 0,
            Timestamp = now
        };
        _db.BusPositionsHistory.Add(history);

        //await _db.SaveChangesAsync();

        // Emitir por SignalR
        await _hub.Clients.All.SendAsync("ReceivePosition", new
        {
            busId = dto.BusId,
            lat = dto.Latitude,
            lng = dto.Longitude,
            speed = dto.SpeedKph,
            timestamp = now
        });

        return Ok(new { success = true });
    }
}
