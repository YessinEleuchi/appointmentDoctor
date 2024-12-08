
namespace AppointmentDoctor.Models.Reposotries
{
    public interface ISpecialityRepository
    {
        Task<IEnumerable<Speciality>> GetAllAsync();
        Task<Speciality> GetByIdAsync(int id);
        Task AddAsync(Speciality speciality);
        Task UpdateAsync(Speciality speciality);
        Task DeleteAsync(int id);
    }

}