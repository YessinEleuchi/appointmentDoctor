using AppointmentDoctor.Models.Reposotries.Interfaces;
using AppointmentDoctor.Models.Reposotries;
using AppointmentDoctor.DTO;
using AppointmentDoctor.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using AutoMapper;

namespace AppointmentDoctor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MedicalHistoryController : ControllerBase
    {
        private readonly IMedicalHistoryRepository medicalHistoryRepository;
        private readonly IMapper mapper;

        public MedicalHistoryController(IMedicalHistoryRepository medicalHistoryRepository , IMapper mapper)
        {
            this.medicalHistoryRepository = medicalHistoryRepository;
            this.mapper = mapper;
        }

        [HttpGet("all")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetAllMedicalHistories([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var medicalHistories = await medicalHistoryRepository.GetAllAsync(pageNumber, pageSize);
                return Ok(medicalHistories);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{historyId}")]
        [Authorize]
        public async Task<IActionResult> GetMedicalHistory(int historyId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            try
            {
                var medicalHistory = await medicalHistoryRepository.GetByIdAsync(historyId);
                if (medicalHistory == null)
                {
                    return NotFound();
                }

                if (medicalHistory.UserId != userId && !User.IsInRole("doctor"))
                {
                    return Unauthorized();
                }
                var medicalHistoryDTO = mapper.Map<CreateMedicalHistoryDTO>(medicalHistory);
                return Ok(medicalHistoryDTO);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("patient/{patientId}")]
        [Authorize(Roles = "doctor,admin")]
        public async Task<IActionResult> GetAllMedicalHistoryByPatientId(string patientId)
        {
            try
            {
                var medicalHistory = await medicalHistoryRepository.GetMedicalHistoryByPatientIdAsync(patientId);
                if (medicalHistory == null)
                {
                    return NotFound();
                }
                var medicalHistoryDTOs = mapper.Map<List<CreateMedicalHistoryDTO>> (medicalHistory);
                return Ok(medicalHistoryDTOs);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateMedicalHistory(CreateMedicalHistoryDTO medicalHistoryDTO)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                    // Check if the patient already has a medical history
                    var existingMedicalHistory = await medicalHistoryRepository.GetMedicalHistoryByPatientIdAsync(userId);

                    // If the patient already has a medical history, return a BadRequest
                    if (existingMedicalHistory != null && existingMedicalHistory.Any())
                    {
                        return BadRequest("A medical history already exists for this patient.");
                    }

                    // Map the DTO to the MedicalHistory entity and set the UserId
                    var medicalHistory = mapper.Map<MedicalHistory>(medicalHistoryDTO);
                    medicalHistory.UserId = userId;

                    // Create the new medical history
                    await medicalHistoryRepository.CreateAsync(medicalHistory);

                    // Return the newly created medical history with a 201 status
                    return CreatedAtAction(nameof(GetMedicalHistory), new { historyId = medicalHistory.MedicalHistoryID }, medicalHistory);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }

            return BadRequest(ModelState);
        }


        [HttpPut("{historyId}")]
        [Authorize]
        public async Task<IActionResult> UpdateMedicalHistory(int historyId, UpdateMedicalHistoryDTO medicalHistoryDTO)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    var currentHistory = await medicalHistoryRepository.GetByIdAsync(historyId);

                    if (currentHistory == null)
                    {
                        return NotFound();
                    }

                    if (!currentHistory.UserId.Equals(userId))
                    {
                        return Unauthorized();
                    }


                    mapper.Map(medicalHistoryDTO, currentHistory);
                    await medicalHistoryRepository.UpdateAsync(currentHistory);
                    return Ok(currentHistory);
                  
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        [HttpDelete("{historyId}")]
        [Authorize]
        public async Task<IActionResult> DeleteMedicalHistory(int historyId)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var currentHistory = await medicalHistoryRepository.GetByIdAsync(historyId);

                if (currentHistory == null)
                {
                    return NotFound();
                }

                if (!currentHistory.UserId.Equals(userId))
                {
                    return Unauthorized();
                }

                await medicalHistoryRepository.DeleteAsync(historyId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
