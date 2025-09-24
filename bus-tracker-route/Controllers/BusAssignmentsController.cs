using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BusTracker.Data;
using BusTracker.Models;
using bus_tracker_route.Models.DTO;

namespace BusTracker.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BusAssignmentsController : ControllerBase
    {
        private readonly AppDbContext _db;

        public BusAssignmentsController(AppDbContext db)
        {
            _db = db;
        }

        // GET /busassignments
        // Lista todas las asignaciones de buses con su ruta y conductor
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var assignments = await _db.BusAssignments
            .Select(a => new BusAssignmentDto
            {
                Id = a.Id,
                BusId = a.BusId,
                BusRouteId = a.BusRouteId,
                DriverId = a.DriverId,
                StartTime = a.StartTime,
                EndTime = a.EndTime
            }).ToListAsync();

            return Ok(assignments);

        }

        // GET /busassignments/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var assignment = await _db.BusAssignments
                .Include(a => a.Bus)
                .Include(a => a.BusRoute)
                .Include(a => a.Driver)
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == id);

            if (assignment == null) return NotFound();

            return Ok(assignment);
        }

        // POST /busassignments
        // Crear nueva asignación de bus a ruta (y opcionalmente a conductor)
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BusAssignment assignment)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            _db.BusAssignments.Add(assignment);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = assignment.Id }, assignment);
        }

        // PUT /busassignments/5
        // Actualizar asignación existente
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] BusAssignment updated)
        {
            var assignment = await _db.BusAssignments.FindAsync(id);
            if (assignment == null) return NotFound();

            assignment.BusId = updated.BusId;
            assignment.BusRouteId = updated.BusRouteId;
            assignment.DriverId = updated.DriverId;
            assignment.StartTime = updated.StartTime;
            assignment.EndTime = updated.EndTime;

            await _db.SaveChangesAsync();
            return NoContent();
        }

        // DELETE /busassignments/5
        // Eliminar una asignación
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var assignment = await _db.BusAssignments.FindAsync(id);
            if (assignment == null) return NotFound();

            _db.BusAssignments.Remove(assignment);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
