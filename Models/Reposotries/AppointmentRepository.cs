using AppointmentDoctor.DTO;
using AppointmentDoctor.Models.Reposotries.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AppointmentDoctor.Models.Reposotries
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly AppDbContext context;

        public AppointmentRepository(AppDbContext context)
        {
            this.context = context;
        }

        public async Task AddAsync(Appointment appointment)
        {
            await context.appointments.AddAsync(appointment);
            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int appointmentId, string userId)
        {
            var appointment = await context.appointments.FirstOrDefaultAsync(a => a.AppointmentID == appointmentId);
            if (appointment != null)
            {
                if (appointment.PatientId == userId || appointment.DoctorId == userId)
                {
                    context.appointments.Remove(appointment);
                    await context.SaveChangesAsync();
                }
                else
                {
                    throw new UnauthorizedAccessException("You are not allowed to delete this appointment.");
                }
            }
            else
            {
                throw new ArgumentException("Appointment not found.");
            }
        }

        public async Task<Appointment?> GetByIdAsync(int appointmentId)
        {
            return await context.appointments.FirstOrDefaultAsync(a => a.AppointmentID == appointmentId);
        }

        public async Task<IEnumerable<Appointment>> GetAvailableBySpecialityAsync(string speciality, int pageNumber, int pageSize)
        {
            var appointments = context.appointments
                .Include(a => a.Doctor)
                .Where(a => a.Status.Equals("Available") && a.AppointmentDateTime > DateTime.Now)
                .Where(a => a.Doctor.Speciality.Equals(speciality, StringComparison.OrdinalIgnoreCase));

            return await appointments
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetAllAsync(int pageNumber, int pageSize)
        {
            return await context.appointments
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetAvailableAsync(int pageNumber, int pageSize)
        {
            return await context.appointments.Include(a => a.Doctor)
                .Where(a => a.Status.Equals("Available") && a.AppointmentDateTime > DateTime.Now)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task UpdateAsync(Appointment appointment)
        {
            var currentAppointment = await context.appointments.FirstOrDefaultAsync(a => a.AppointmentID == appointment.AppointmentID);
            if (currentAppointment != null)
            {
                currentAppointment.AppointmentDateTime = appointment.AppointmentDateTime;
                currentAppointment.Status = appointment.Status;
                currentAppointment.PatientId = appointment.PatientId;
                currentAppointment.DoctorId = appointment.DoctorId;
                currentAppointment.Notes = appointment.Notes;

                await context.SaveChangesAsync();
            }
            else
            {
                throw new ArgumentException("Appointment not found.");
            }
        }

        public async Task BookAsync(BookAppointmentDTO appointment)
        {
            var currentAppointment = await context.appointments.FirstOrDefaultAsync(a => a.AppointmentID == appointment.AppointmentID);
            if (currentAppointment != null)
            {
                currentAppointment.Status = "Scheduled";
                currentAppointment.PatientId = appointment.PatientId;

                await context.SaveChangesAsync();
            }
            else
            {
                throw new ArgumentException("Appointment not found.");
            }
        }

        public async Task CancelAsync(int appointmentId, string userId)
        {
            var currentAppointment = await context.appointments.FirstOrDefaultAsync(a => a.AppointmentID == appointmentId);
            if (currentAppointment != null)
            {
                if (currentAppointment.PatientId == userId || currentAppointment.DoctorId == userId)
                {
                    currentAppointment.Status = "Canceled";
                    currentAppointment.Notes = (currentAppointment.Notes ?? string.Empty) + $"\n[Appointment canceled by user: {userId}]";
                    await context.SaveChangesAsync();
                }
                else
                {
                    throw new UnauthorizedAccessException("You are not authorized to cancel this appointment.");
                }
            }
            else
            {
                throw new ArgumentException("Appointment not found.");
            }
        }

        public async Task<IEnumerable<Appointment>> GetScheduledByPatientIdAsync(string patientId, int pageNumber, int pageSize)
        {
            return await context.appointments.Include(a => a.Doctor)
                .Where(a => a.PatientId == patientId && a.Status == "Scheduled")
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
    }
}
