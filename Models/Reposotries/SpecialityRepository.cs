using Microsoft.EntityFrameworkCore;

namespace AppointmentDoctor.Models.Reposotries
{
    public class SpecialityRepository : ISpecialityRepository
    {
        private readonly AppDbContext _context;

        public SpecialityRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Speciality>> GetAllAsync()
        {
            return await _context.Specialities.ToListAsync();
        }

        public async Task<Speciality> GetByIdAsync(int id)
        {
            return await _context.Specialities.FindAsync(id);
        }

        public async Task AddAsync(Speciality speciality)
        {
            await _context.Specialities.AddAsync(speciality);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Speciality speciality)
        {
            _context.Specialities.Update(speciality);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var speciality = await _context.Specialities.FindAsync(id);
            if (speciality != null)
            {
                _context.Specialities.Remove(speciality);
                await _context.SaveChangesAsync();
            }
        }
    }

}
