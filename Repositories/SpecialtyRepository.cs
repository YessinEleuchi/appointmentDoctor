using AppointmentDoctor.Models;
using Microsoft.EntityFrameworkCore;

namespace AppointmentDoctor.Repositories
{
    public class SpecialtyRepository : ISpecialtyRepository
    {
        private readonly AppDbContext _context;

        public SpecialtyRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Specialty>> GetAllSpecialtiesAsync()
        {
            return await _context.Specialties.ToListAsync();
        }

        public async Task<Specialty> GetSpecialtyByIdAsync(int id)
        {
            return await _context.Specialties.FindAsync(id);
        }

        public async Task AddSpecialtyAsync(Specialty specialty)
        {
            _context.Specialties.Add(specialty);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateSpecialtyAsync(Specialty specialty)
        {
            _context.Specialties.Update(specialty);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteSpecialtyAsync(int id)
        {
            var specialty = await _context.Specialties.FindAsync(id);
            if (specialty != null)
            {
                _context.Specialties.Remove(specialty);
                await _context.SaveChangesAsync();
            }
        }
    }
}
