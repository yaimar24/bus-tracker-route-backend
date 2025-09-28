using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BusTracker.Data;
using BusTracker.Models;

namespace BusTracker.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DriversController : ControllerBase
    {
        private readonly AppDbContext _db;

        public DriversController(AppDbContext db)
        {
            _db = db;
        }

        // GET /drivers?companyId=1
        // Lista todos los conductores, con filtro opcional por empresa
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int? companyId)
        {
            var query = _db.Drivers
                .Include(d => d.Company)
                .Include(d => d.Assignments)
                    .ThenInclude(a => a.BusRoute)
                .AsQueryable();

            if (companyId.HasValue)
                query = query.Where(d => d.CompanyId == companyId.Value);

            var drivers = await query.AsNoTracking().ToListAsync();
            return Ok(drivers);
        }

        // GET /drivers/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var driver = await _db.Drivers
                .Include(d => d.Company)
                .Include(d => d.Assignments)
                    .ThenInclude(a => a.BusRoute)
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == id);

            if (driver == null) return NotFound();

            return Ok(driver);
        }

        // POST /drivers
        // Registrar un nuevo conductor
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Driver driver)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            _db.Drivers.Add(driver);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = driver.Id }, driver);
        }

        // PUT /drivers/5
        // Actualizar datos de un conductor
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Driver updated)
        {
            var driver = await _db.Drivers.FindAsync(id);
            if (driver == null) return NotFound();

            driver.Name = updated.Name;
            driver.LicenseNumber = updated.LicenseNumber;
            driver.CompanyId = updated.CompanyId;

            await _db.SaveChangesAsync();
            return NoContent();
        }

        // DELETE /drivers/5
        // Eliminar un conductor
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var driver = await _db.Drivers.FindAsync(id);
            if (driver == null) return NotFound();

            _db.Drivers.Remove(driver);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
