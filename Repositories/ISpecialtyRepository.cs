using AppointmentDoctor.Models;

namespace AppointmentDoctor.Repositories
{
    public interface ISpecialtyRepository
    {
        Task<IEnumerable<Specialty>> GetAllSpecialtiesAsync();
        Task<Specialty> GetSpecialtyByIdAsync(int id);
        Task AddSpecialtyAsync(Specialty specialty);
        Task UpdateSpecialtyAsync(Specialty specialty);
        Task DeleteSpecialtyAsync(int id);
    }
}
