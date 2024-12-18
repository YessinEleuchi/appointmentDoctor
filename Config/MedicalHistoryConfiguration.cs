using AppointmentDoctor.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace AppointmentDoctor.Config
{
    public class MedicalHistoryConfiguration : IEntityTypeConfiguration<MedicalHistory>
    {
        public void Configure(EntityTypeBuilder<MedicalHistory> builder)
        {
            builder.ToTable("MedicalHistories");

            builder.HasKey(mh => mh.MedicalHistoryID);

            // Configure the foreign key relationship with ApplicationUser (User)
            builder.HasOne(mh => mh.Patient)
                .WithMany(u => u.MedicalHistories)
                .HasForeignKey(mh => mh.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure the DateOfEntry property to be a required field with a default value
            builder.Property(mh => mh.DateOfEntry)
            .IsRequired()
            .HasDefaultValueSql("GETDATE()");
        }
    }
}
