using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BusTracker.Data;
using BusTracker.Models;
using bus_tracker_route.Models.DTO;

namespace BusTracker.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CompaniesController : ControllerBase
    {
        private readonly AppDbContext _db;

        public CompaniesController(AppDbContext db)
        {
            _db = db;
        }

        // GET: api/companies
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var companies = await _db.Companies.AsNoTracking().ToListAsync();
            return Ok(companies);
        }

        // GET: api/companies/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var company = await _db.Companies.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
            if (company == null) return NotFound();
            return Ok(company);
        }

        // POST: api/companies
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CompanyDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                return BadRequest("El nombre de la compañía es obligatorio.");

            var company = new Company
            {
                Name = dto.Name,
                ContactInfo = dto.ContactInfo
            };

            _db.Companies.Add(company);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = company.Id }, company);
        }

        // PUT: api/companies/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CompanyDto dto)
        {
            var company = await _db.Companies.FindAsync(id);
            if (company == null) return NotFound();

            company.Name = dto.Name;
            company.ContactInfo = dto.ContactInfo;

            await _db.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/companies/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var company = await _db.Companies.FindAsync(id);
            if (company == null) return NotFound();

            _db.Companies.Remove(company);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
