using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace AppointmentDoctor.Config
{
    public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
    {
        public void Configure(EntityTypeBuilder<Appointment> builder)
        {
            builder.ToTable("Appointments");

            builder.HasKey(a => a.AppointmentID);

            builder.Property(a => a.AppointmentDateTime)
                .IsRequired();

            builder.Property(a => a.Status)
                .HasDefaultValue("Available")
                .HasMaxLength(20);

            builder.Property(a => a.Notes)
                .HasMaxLength(250);

            // Configure the relationship with the Doctor
            builder.HasOne(a => a.Doctor)
                .WithMany()
                .HasForeignKey(a => a.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure the relationship with the Patient
            builder.HasOne(a => a.Patient)
                .WithMany()
                .HasForeignKey(a => a.PatientId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
