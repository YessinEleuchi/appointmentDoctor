namespace AppointmentDoctor.Models.Reposotries.Interfaces
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