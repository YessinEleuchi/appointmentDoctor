using AppointmentDoctor.Models;
using AppointmentDoctor.Models.Reposotries.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentDoctor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "admin")]
    public class SpecialityController : ControllerBase
    {
        private readonly ISpecialityRepository _repository;

        public SpecialityController(ISpecialityRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var specialities = await _repository.GetAllAsync();
            return Ok(specialities);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var speciality = await _repository.GetByIdAsync(id);
            if (speciality == null)
            {
                return NotFound();
            }
            return Ok(speciality);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Speciality speciality)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _repository.AddAsync(speciality);
            return CreatedAtAction(nameof(GetById), new { id = speciality.Id }, speciality);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Speciality speciality)
        {
            if (id != speciality.Id)
            {
                return BadRequest("ID mismatch");
            }

            await _repository.UpdateAsync(speciality);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _repository.DeleteAsync(id);
            return NoContent();
        }
    }

}
