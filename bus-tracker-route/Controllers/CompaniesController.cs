using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BusTracker.Data;
using BusTracker.Models;

namespace BusTracker.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CompaniesController : ControllerBase
    {
        private readonly AppDbContext _db;

        public CompaniesController(AppDbContext db)
        {
            _db = db;
        }

        // GET /companies
        // Lista todas las empresas
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var companies = await _db.Companies
                .AsNoTracking()
                .ToListAsync();

            return Ok(companies);
        }

        // GET /companies/5
        // Detalle de una empresa
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var company = await _db.Companies
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);

            if (company == null) return NotFound();

            return Ok(company);
        }

        // GET /companies/5/buses
        // Lista todos los buses de una empresa, incluyendo su ruta asignada
        [HttpGet("{id}/buses")]
        public async Task<IActionResult> GetBusesByCompany(int id)
        {
            var buses = await _db.Buses
                .Where(b => b.CompanyId == id)
                .Include(b => b.Assignments)
                    .ThenInclude(a => a.BusRoute)
                .AsNoTracking()
                .ToListAsync();

            if (buses.Count == 0) return NotFound("No hay buses registrados para esta empresa.");

            return Ok(buses);
        }
    }
}
