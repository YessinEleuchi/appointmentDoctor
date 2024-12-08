namespace AppointmentDoctor.Models.Reposotries.Interfaces
{
    public interface IMedicalHistoryRepository
    {
        Task<IEnumerable<MedicalHistory>> GetAllAsync(int pageNumber, int pageSize);
        Task<MedicalHistory?> GetByIdAsync(int medicalHistoryId);
        Task<IEnumerable<MedicalHistory>?> GetMedicalHistoryByPatientIdAsync(string patientId);
        Task CreateAsync(MedicalHistory medicalHistory);
        Task UpdateAsync(MedicalHistory medicalHistory);
        Task DeleteAsync(int medicalHistoryId);
    }
}
