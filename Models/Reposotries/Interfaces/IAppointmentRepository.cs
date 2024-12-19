using AppointmentDoctor.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppointmentDoctor.Models.Reposotries.Interfaces
{
    public interface IAppointmentRepository
    {
        Task AddAsync(Appointment appointment);
        Task DeleteAsync(int appointmentId, string userId);
        Task<Appointment?> GetByIdAsync(int appointmentId);
        Task<IEnumerable<Appointment>> GetAvailableBySpecialityAsync(string speciality, int pageNumber, int pageSize);
        Task<IEnumerable<Appointment>> GetAllAsync(int pageNumber, int pageSize);
        Task<IEnumerable<Appointment>> GetAvailableAsync(int pageNumber, int pageSize);
        Task<IEnumerable<Appointment>> GetScheduledByPatientIdAsync(string patientId, int pageNumber, int pageSize);
        Task<IEnumerable<Appointment>> GetScheduledByDoctorIdAsync(string doctorId, int pageNumber, int pageSize); // Nouvelle méthode
        Task<string?> GetAppointmentDocumentPathAsync(int appointmentId); // Nouvelle méthode
        Task UpdateAsync(Appointment appointment);
        Task BookAsync(BookAppointmentDTO appointment);
        Task CancelAsync(int appointmentId, string userId);
        Task<IEnumerable<Appointment>> GetAvailableAppointmentsByDoctorIdAsync(string doctorId);

    }
}
