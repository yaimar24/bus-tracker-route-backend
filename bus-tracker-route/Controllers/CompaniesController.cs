using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BusTracker.Data;
using BusTracker.Models;
using bus_tracker_route.Models.DTO;

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
        [HttpGet("{companyId}/routes")]
        public async Task<IActionResult> GetRoutesByCompany(int companyId)
        {
            var routes = await _db.BusAssignments
                .Where(a => a.Bus.CompanyId == companyId && a.IsActive)
                .Select(a => a.BusRoute)
                .Distinct()
                .AsNoTracking()
                .ToListAsync();

            return Ok(routes);
        }

    }
}
