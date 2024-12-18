using AppointmentDoctor.DTO;
using AppointmentDoctor.Models.Reposotries.Interfaces;
using AppointmentDoctor.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using AutoMapper;



namespace AppointmentDoctor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentsController : ControllerBase
    {
        private readonly IAppointmentRepository appointmentRepository;
        private readonly AppDbContext context;
        private readonly IMapper mapper;


        public AppointmentsController(IAppointmentRepository appointmentRepository, AppDbContext context, IMapper mapper)
        {
            this.appointmentRepository = appointmentRepository;
            this.context = context;
            this.mapper = mapper; // Initialisation de l'instance IMapper

        }

        /// <summary>
        /// Get a paginated list of all appointments with error handling, Requires Admin Access.
        /// </summary>
        /// <param name="pageNumber">The page number (default is 1).</param>
        /// <param name="pageSize">The number of items per page (default is 10).</param>
        /// <returns>A paginated list of appointments or an error response if an exception occurs.</returns>

        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetAllAppointments(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10
            )
        {
            try
            {
                var appointments = await appointmentRepository.GetAllAsync(pageNumber, pageSize);

                if (appointments == null)
                    return NotFound();

                var appointmentDTOs = mapper.Map<List<AppointmentDTO>>(appointments);
                foreach (var item in appointments)
                {
                    appointmentDTOs.Add(AppointmentDTO.FromAppointment(item));
                }
                return Ok(appointmentDTOs);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while fetching appointments.");
            }
        }

        /// <summary>
        /// Get an appointment by its unique identifier.
        /// </summary>
        /// <param name="appointmentId">The ID of the appointment to retrieve.</param>
        /// <returns>
        /// Returns the appointment with the specified ID if found and the user is authorized;
        /// otherwise, returns NotFound or Unauthorized as appropriate.
        /// </returns>
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetAppointment(int appointmentId)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var appointment = await appointmentRepository.GetByIdAsync(appointmentId);
                if (appointment == null)
                {
                    return NotFound();
                }

                if (!User.IsInRole("Admin") && !appointment.PatientId.Equals(userId) && !appointment.DoctorId.Equals(userId))
                {
                    return Unauthorized("You Are Not Authorized to View this Appointment");
                }

                AppointmentDTO appointmentDTO = mapper.Map<AppointmentDTO>(appointment);
                return Ok(appointmentDTO);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Get a paginated list of available appointments for scheduling.
        /// </summary>
        /// <param name="pageNumber">The page number for pagination (default is 1).</param>
        /// <param name="pageSize">The number of appointments to include per page (default is 10).</param>
        /// <returns>A paginated list of available appointments.</returns>
        // GET: api/appointments/Available?pageNumber=2&pageSize=20
        [HttpGet("Available")]
        public async Task<IActionResult> GetAvailableAppointments(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10
            )
        {
            try
            {
                var appointments = await appointmentRepository.GetAvailableAsync(pageNumber, pageSize);

                if (appointments == null)
                    return NotFound();

                var appointmentDTOs = mapper.Map<List<AppointmentDTO>>(appointments);
                foreach (var item in appointments)
                {
                    appointmentDTOs.Add(AppointmentDTO.FromAppointment(item));
                }

                return Ok(appointmentDTOs);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Get a paginated list of available appointments for a specific medical specialty.
        /// </summary>
        /// <param name="speciality">The medical specialty to filter appointments by.</param>
        /// <param name="pageNumber">The page number for pagination (default is 1).</param>
        /// <param name="pageSize">The number of appointments to include per page (default is 10).</param>
        /// <returns>A paginated list of available appointments for the specified specialty.</returns>
        // GET: api/appointments/Available/BySpeciality/{speciality}?pageNumber=2&pageSize=20
        [HttpGet("Available/BySpeciality/{speciality}")]
        public async Task<IActionResult> GetAvailableAppointmentsBySpeciality(
            [FromRoute] string speciality,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10
            )
        {
            try
            {
                var appointments = await appointmentRepository.GetAvailableBySpecialityAsync(speciality, pageNumber, pageSize);

                if (appointments == null)
                    return NotFound();

                var appointmentDTOs = mapper.Map<List<AppointmentDTO>>(appointments);
                foreach (var item in appointments)
                {
                    appointmentDTOs.Add(AppointmentDTO.FromAppointment(item));
                }

                return Ok(appointmentDTOs);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        /// <summary>
        /// Get a paginated list of scheduled appointments for the specified patient.
        /// </summary>
        /// <param name="patientId">The ID of the patient whose appointments are being retrieved.</param>
        /// <param name="pageNumber">The page number for pagination (default is 1).</param>
        /// <param name="pageSize">The number of appointments to include per page (default is 10).</param>
        /// <returns>
        /// A paginated list of scheduled appointments for the specified patient, 
        /// or an error message if unauthorized or no appointments are found.
        /// </returns>
        [HttpGet("my-appointments")]
        [Authorize]
        public async Task<IActionResult> GetScheduledAppointmentsByPatient(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10
            )
        {
            try
            {

                var UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                // Fetch the scheduled appointments for the specified patient
                var appointments = await appointmentRepository.GetScheduledByPatientIdAsync(UserId, pageNumber, pageSize);

                // Check if appointments are found
                if (appointments.Any())
                {
                    var appointmentDTOs = mapper.Map<List<AppointmentDTO>>(appointments);
                    foreach (var item in appointments)
                    {
                        appointmentDTOs.Add(AppointmentDTO.FromAppointment(item));
                    }

                    return Ok(appointmentDTOs);
                }

                // No appointments found
                return NotFound(new { message = "No scheduled appointments found for the specified patient." });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        /// <summary>
        /// Create a new appointment by a medical professional for a patient.
        /// </summary>
        /// <param name="appointmentDTO">The appointment details to be created.</param>
        /// <returns>
        /// Returns a 201 Created response with the created appointment's details if successful.
        /// Returns a Bad Request (400) response with validation errors if the model state is invalid.
        /// Returns a Bad Request (400) response with an error message if an error occurs during creation.
        /// </returns>
        // POST: api/appointments
        [HttpPost]
        [Authorize(Roles = "doctor")]
        public async Task<IActionResult> CreateAppointment(CreateAppointmentDTO appointmentDTO)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var doctorId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                    // Créez le rendez-vous
                    var appointment = new Appointment
                    {
                        AppointmentDateTime = appointmentDTO.AppointmentDateTime,

                        Status = "Available",
                        DoctorId = doctorId,
                    };

                    await appointmentRepository.AddAsync(appointment);

                    // Utilisez la méthode du contexte pour obtenir le nom d'utilisateur du docteur
                    var doctorUsername = await context.GetDoctorUsernameAsync(doctorId);

                    return CreatedAtAction(nameof(GetAppointment), new
                    {
                        id = appointment.AppointmentID
                    },
                    new
                    {
                        appointment.AppointmentID,
                        appointment.AppointmentDateTime,
                        appointment.Status,
                        DoctorUsername = doctorUsername
                    });
                }

                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while creating the appointment: {ex.Message}");
            }
        }



        /// <summary>
        /// Book an available appointment by a patient.
        /// </summary>
        /// <param name="id">The ID of the appointment to be booked.</param>
        /// <param name="appointment">The appointment details including the appointment ID.</param>
        /// <returns>
        /// Returns an OK (200) response with a success message if the appointment is booked successfully.
        /// Returns a Bad Request (400) response with an "ID mismatch" message if the provided ID and appointment ID do not match.
        /// Returns a Bad Request (400) response with validation errors if the model state is invalid.
        /// Returns a Bad Request (400) response with an error message if an error occurs during booking.
        /// </returns>
        // PUT: api/appointments/Book/1
        [HttpPut("Book/{id}")]
        [Authorize]
        public async Task<IActionResult> BookAppointment(int id, BookAppointmentDTO appointment)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { error = string.Join(", ", ModelState.Values.Select(v => v.Errors.Select(e => e.ErrorMessage))) });
                }

                var patientId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (patientId is null)
                {
                    return Unauthorized(new { error = "Please log in again." });
                }

                // Retrieve the appointment from the database
                var existingAppointment = await appointmentRepository.GetByIdAsync(id);
                if (existingAppointment == null)
                {
                    return NotFound(new { error = "Appointment not found." });
                }

                // Check if the appointment is available
                if (existingAppointment.Status != "Available")
                {
                    return BadRequest(new { error = "The appointment is not available for booking." });
                }

                // Update the appointment details
                existingAppointment.PatientId = patientId; // Assign the current user as the patient
                existingAppointment.Status = "Scheduled"; // Update the status to "Scheduled"

                // Save the changes to the database
                await appointmentRepository.UpdateAsync(existingAppointment);

                return Ok(new { message = "Appointment booked successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = "Something went wrong, please try again later." });
            }
        }

        /// <summary>
        /// Cancel an existing appointment by the patient or doctor.
        /// </summary>
        /// <param name="appointmentId">The ID of the appointment to be canceled.</param>
        /// <returns>
        /// Returns a No Content (204) response if the appointment is canceled successfully.
        /// Returns a Bad Request (400) response with an error message if an error occurs during cancellation.
        /// </returns>
        // Put: api/appointments/1
        [HttpPut("{appointmentId}")]
        [Authorize]
        public async Task<IActionResult> CancelAppointment(int appointmentId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            try
            {
                await appointmentRepository.CancelAsync(appointmentId, userId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Delete an existing appointment by the patient or doctor.
        /// </summary>
        /// <param name="id">The ID of the appointment to be deleted.</param>
        /// <returns>
        /// Returns a No Content (204) response if the appointment is deleted successfully.
        /// Returns a Bad Request (400) response with an error message if an error occurs during deletion.
        /// </returns>
        // DELETE: api/appointments/1
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteAppointment(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            try
            {
                await appointmentRepository.DeleteAsync(id, userId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}