using AppointmentDoctor.Models;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

public class ApplicationUser : IdentityUser
{
    [Required]
    [MaxLength(50)]
    public string FirstName { get; set; }

    [Required]
    [MaxLength(50)]
    public string LastName { get; set; }


    [Required]
    public string Adress { get; set; }
    public int? SpecialtyId { get; set; } // Foreign key for Specialty

    [ForeignKey("SpecialtyId")]
    public Speciality? Specialty { get; set; } // Navigation property

    public ICollection<MedicalHistory> MedicalHistories { get; set; } // For patients
}
