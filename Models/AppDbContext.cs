using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AppointmentDoctor.Models
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions options) : base(options) { }

        public DbSet<Specialty> Specialties { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configurer les tables séparées pour Patient et Doctor
            builder.Entity<Patient>()
                .ToTable("Patients");  // Table spécifique pour Patient

            builder.Entity<Doctor>()
                .ToTable("Doctors");  // Table spécifique pour Doctor
        }
    }
}
