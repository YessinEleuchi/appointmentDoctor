using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using AppointmentDoctor.Models;
using AppointmentDoctor.Config;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // DbSet properties
    public DbSet<MedicalHistory> medicalHistories { get; set; }
    public DbSet<Appointment> appointments { get; set; }

    // Configure entity mappings
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply entity configurations
        modelBuilder.ApplyConfiguration(new AppointmentConfiguration());
    }

    /// <summary>
    /// Retrieves the username of a doctor based on their ID.
    /// </summary>
    /// <param name="doctorId">The ID of the doctor.</param>
    /// <returns>The username of the doctor, or null if not found.</returns>
    public async Task<string?> GetDoctorUsernameAsync(string doctorId)
    {
        if (string.IsNullOrWhiteSpace(doctorId))
            throw new ArgumentException("Doctor ID cannot be null or empty.", nameof(doctorId));

        return await Users
            .Where(user => user.Id == doctorId)
            .Select(user => user.UserName)
            .FirstOrDefaultAsync();
    }
}
