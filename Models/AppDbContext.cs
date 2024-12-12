using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using AppointmentDoctor.Models;
using AppointmentDoctor.Config;  // Make sure to import the correct namespace for AppointmentConfiguration

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<MedicalHistory> medicalHistories { get; set; }
    public DbSet<Appointment> appointments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply the configuration for Appointment entity
        modelBuilder.ApplyConfiguration(new AppointmentConfiguration());
    }
    public async Task<string?> GetDoctorUsernameAsync(string doctorId)
    {
        // Vérifie si l'ID du docteur est valide et retourne le UserName
        return await Users
            .Where(user => user.Id == doctorId)
            .Select(user => user.UserName)
            .FirstOrDefaultAsync();
    }

}
