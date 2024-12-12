using AppointmentDoctor.DTO;

namespace AppointmentDoctor.Models.Reposotries.Interfaces
{
    public interface IAppointmentRepository
    {
        Task<IEnumerable<Appointment>> GetAllAsync(int pageNumber, int pageSize);
        Task<IEnumerable<Appointment>> GetAvailableAsync(int pageNumber, int pageSize);//status = available
        Task<IEnumerable<Appointment>> GetAvailableBySpecialityAsync(string speciality, int pageNumber, int pageSize);
        Task<IEnumerable<Appointment>> GetScheduledByPatientIdAsync(string patientId, int pageNumber, int pageSize);
        Task<Appointment?> GetByIdAsync(int appointmentId);
        Task AddAsync(Appointment appointment);
        Task UpdateAsync(Appointment appointment);
        Task BookAsync(BookAppointmentDTO appointment);
        Task CancelAsync(int appointmentId, string userId);
        Task DeleteAsync(int appointmentId, string userId);

    }
}
