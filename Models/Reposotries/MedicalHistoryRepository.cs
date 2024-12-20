﻿using AppointmentDoctor.Models.Reposotries.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AppointmentDoctor.Models.Reposotries
{
    public class MedicalHistoryRepository : IMedicalHistoryRepository
    {
        private readonly AppDbContext context;

        public MedicalHistoryRepository(AppDbContext context)
        {
            this.context = context;
        }

        public async Task CreateAsync(MedicalHistory medicalHistory)
        {
            context.medicalHistories.Add(medicalHistory);
            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int medicalHistoryId)
        {
            var history = await context.medicalHistories.FirstOrDefaultAsync(mh => mh.MedicalHistoryID == medicalHistoryId);
            if (history != null)
            {
                context.medicalHistories.Remove(history);
                await context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<MedicalHistory>> GetAllAsync(int pageNumber, int pageSize)
        {
            if (pageNumber <= 0 || pageSize <= 0)
            {
                throw new ArgumentException("Invalid page number or page size.");
            }

            int itemsToSkip = (pageNumber - 1) * pageSize;

            var medicalHistories = await context.medicalHistories
                .Skip(itemsToSkip)
                .Take(pageSize)
                .ToListAsync();

            return medicalHistories;
        }

        public async Task<MedicalHistory?> GetByIdAsync(int medicalHistoryId)
        {
            return await context.medicalHistories.FirstOrDefaultAsync(mh => mh.MedicalHistoryID == medicalHistoryId);
        }

        public async Task<IEnumerable<MedicalHistory>?> GetMedicalHistoryByPatientIdAsync(string patientId)
        {
            return await context.medicalHistories.Where(mh => mh.UserId == patientId).ToListAsync();
        }

        public async Task UpdateAsync(MedicalHistory medicalHistory)
        {
            context.medicalHistories.Update(medicalHistory);
            await context.SaveChangesAsync();
        }
    }
}
