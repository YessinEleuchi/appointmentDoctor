using AppointmentDoctor.Models;
using AppointmentDoctor.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentDoctor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "admin")]
    public class SpecialtyController : ControllerBase
    {
        private readonly ISpecialtyRepository _specialtyRepository;

        public SpecialtyController(ISpecialtyRepository specialtyRepository)
        {
            _specialtyRepository = specialtyRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSpecialties()
        {
            var specialties = await _specialtyRepository.GetAllSpecialtiesAsync();
            return Ok(specialties);
        }

        [HttpPost]
        public async Task<IActionResult> AddSpecialty(Specialty specialty)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _specialtyRepository.AddSpecialtyAsync(specialty);
            return Ok("Specialty added successfully.");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSpecialty(int id, Specialty specialty)
        {
            if (id != specialty.Id)
                return BadRequest("Specialty ID mismatch.");

            await _specialtyRepository.UpdateSpecialtyAsync(specialty);
            return Ok("Specialty updated successfully.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSpecialty(int id)
        {
            await _specialtyRepository.DeleteSpecialtyAsync(id);
            return Ok("Specialty deleted successfully.");
        }
    }
}
