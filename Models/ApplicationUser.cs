using AppointmentDoctor.Models;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

public class ApplicationUser : IdentityUser
{
    [Required(ErrorMessage = "Le prénom est requis.")]
    [MaxLength(50, ErrorMessage = "Le prénom ne peut pas dépasser 50 caractères.")]
    public string FirstName { get; set; }

    [Required(ErrorMessage = "Le nom de famille est requis.")]
    [MaxLength(50, ErrorMessage = "Le nom de famille ne peut pas dépasser 50 caractères.")]
    public string LastName { get; set; }

    [Required(ErrorMessage = "L'adresse est requise.")]
    [MaxLength(200, ErrorMessage = "L'adresse ne peut pas dépasser 200 caractères.")]
    public string Adress { get; set; }

    [MaxLength(100, ErrorMessage = "La spécialité ne peut pas dépasser 100 caractères.")]
    public string? Speciality { get; set; } // Facultatif, pour les docteurs

    [Required(ErrorMessage = "Le genre est requis.")]
    [RegularExpression("^(Male|Female)$.")]
    public string Gender { get; set; }

    [Required(ErrorMessage = "L'âge est requis.")]
    [Range(0, 120, ErrorMessage = "L'âge doit être compris entre 0 et 120 ans.")]
    public int Age { get; set; }

    [Range(0, 10000, ErrorMessage = "Les frais doivent être compris entre 0 et 10 000.")]
    public int? Fees { get; set; } // Facultatif, pour les docteurs

    [Range(0, 50, ErrorMessage = "L'expérience doit être comprise entre 0 et 50 ans.")]
    public int? Experience { get; set; } // Facultatif, pour les docteurs

    [MaxLength(500)]
    public string? ProfileImagePath { get; set; } // Chemin de l'image
    public ICollection<MedicalHistory> MedicalHistories { get; set; } = new List<MedicalHistory>(); // Pour les patients
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>(); // Pour les rendez-vous


}
