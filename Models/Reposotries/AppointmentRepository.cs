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

            if (appointment == null)
                throw new KeyNotFoundException("Appointment not found.");

            if (appointment.PatientId != userId && appointment.DoctorId != userId)
                throw new UnauthorizedAccessException("You are not allowed to delete this appointment.");

            context.appointments.Remove(appointment);
            await context.SaveChangesAsync();
        }

        public async Task<Appointment?> GetByIdAsync(int appointmentId)
        {
            return await context.appointments
                .Include(a => a.Doctor)
                .Include(a => a.Patient)
                .FirstOrDefaultAsync(a => a.AppointmentID == appointmentId);
        }

        public async Task<IEnumerable<Appointment>> GetAvailableBySpecialityAsync(string speciality, int pageNumber, int pageSize)
        {
            if (string.IsNullOrEmpty(speciality))
                throw new ArgumentException("Speciality must not be null or empty.");

            var query = context.appointments
                .Include(a => a.Doctor)
                .Where(a => a.Status == "Available" && a.AppointmentDateTime > DateTime.Now &&
                            a.Doctor.Speciality.Equals(speciality, StringComparison.OrdinalIgnoreCase));

            return await Paginate(query, pageNumber, pageSize).ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetAllAsync(int pageNumber, int pageSize)
        {
            return await Paginate(context.appointments, pageNumber, pageSize).ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetAvailableAsync(int pageNumber, int pageSize)
        {
            var query = context.appointments.Include(a => a.Doctor)
                .Where(a => a.Status == "Available" && a.AppointmentDateTime > DateTime.Now);

            return await Paginate(query, pageNumber, pageSize).ToListAsync();
        }

        public async Task UpdateAsync(Appointment appointment)
        {
            var currentAppointment = await context.appointments.FindAsync(appointment.AppointmentID);

            if (currentAppointment == null)
                throw new KeyNotFoundException("Appointment not found.");

            currentAppointment.AppointmentDateTime = appointment.AppointmentDateTime;
            currentAppointment.Status = appointment.Status ?? currentAppointment.Status;
            currentAppointment.Notes = appointment.Notes ?? currentAppointment.Notes;
            currentAppointment.DoctorId = appointment.DoctorId ?? currentAppointment.DoctorId;
            currentAppointment.PatientId = appointment.PatientId ?? currentAppointment.PatientId;

            await context.SaveChangesAsync();
        }

        public async Task BookAsync(BookAppointmentDTO appointment)
        {
            var currentAppointment = await context.appointments.FirstOrDefaultAsync(a => a.AppointmentID == appointment.AppointmentID);

            if (currentAppointment == null)
                throw new KeyNotFoundException("Appointment not found.");

            if (currentAppointment.Status != "Available")
                throw new InvalidOperationException("This appointment is not available for booking.");

            currentAppointment.Status = "Scheduled";
            currentAppointment.PatientId = appointment.PatientId;

            await context.SaveChangesAsync();
        }

        public async Task CancelAsync(int appointmentId, string userId)
        {
            var appointment = await context.appointments.FindAsync(appointmentId);

            if (appointment == null)
                throw new KeyNotFoundException("Appointment not found.");

            if (appointment.PatientId != userId && appointment.DoctorId != userId)
                throw new UnauthorizedAccessException("You are not authorized to cancel this appointment.");

            appointment.Status = "Canceled";
            appointment.Notes = (appointment.Notes ?? string.Empty) +
                                $"\n[Appointment canceled by user {userId} at {DateTime.UtcNow} UTC.]";

            await context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Appointment>> GetScheduledByPatientIdAsync(string patientId, int pageNumber, int pageSize)
        {
            var query = context.appointments.Include(a => a.Doctor)
                .Where(a => a.PatientId == patientId && a.Status == "Scheduled");

            return await Paginate(query, pageNumber, pageSize).ToListAsync();
        }

        // New: Get appointments by doctor
        public async Task<IEnumerable<Appointment>> GetScheduledByDoctorIdAsync(string doctorId, int pageNumber, int pageSize)
        {
            var query = context.appointments.Include(a => a.Patient)
                .Where(a => a.DoctorId == doctorId && a.Status == "Scheduled");

            return await Paginate(query, pageNumber, pageSize).ToListAsync();
        }

        // New: Get appointment documents (if stored as paths in Notes)
        public async Task<string?> GetAppointmentDocumentPathAsync(int appointmentId)
        {
            var appointment = await context.appointments.FirstOrDefaultAsync(a => a.AppointmentID == appointmentId);

            return appointment?.Notes?.Contains("Document Path:") == true
                ? appointment.Notes.Split("Document Path:")[1].Trim()
                : null;
        }

        // Méthode utilitaire pour la pagination
        private IQueryable<T> Paginate<T>(IQueryable<T> query, int pageNumber, int pageSize)
        {
            return query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
        }

        public async Task<IEnumerable<Appointment>> GetAvailableAppointmentsByDoctorIdAsync(string doctorId)
        {
            return await context.appointments
                .Where(a => a.DoctorId == doctorId && a.Status == "Available")
                .ToListAsync();
        }
    }
}
