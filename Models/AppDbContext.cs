using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace AppointmentDoctor.Models
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions options) : base(options) { }

        public DbSet<Specialty> Specialties { get; set; } // Ajouter la table Specialties

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configurer la relation entre ApplicationUser et Specialty
            builder.Entity<ApplicationUser>()
                   .HasOne(a => a.Specialty)
                   .WithMany()
                   .HasForeignKey(a => a.SpecialtyId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
