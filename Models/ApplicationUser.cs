using AppointmentDoctor.Models;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using AppointmentDoctor.DTO;
using System.Collections.Generic;

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
    public string? Speciality { get; set; } //for doctors



    public ICollection<MedicalHistory> medicalHistories { get; set; } // For patients
    public ICollection<Appointment> appointments { get; set; } 


}
