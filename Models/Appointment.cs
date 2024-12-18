using System.ComponentModel.DataAnnotations;

public class Appointment
{
    public int AppointmentID { get; set; }

    [Required(ErrorMessage = "L'ID du docteur est requis.")]
    public string DoctorId { get; set; }

    public ApplicationUser Doctor { get; set; } // Relation de navigation avec le docteur

    public string? PatientId { get; set; }

    public ApplicationUser? Patient { get; set; } // Relation de navigation avec le patient

    [Required(ErrorMessage = "La date et l'heure du rendez-vous sont requises.")]
    public DateTime AppointmentDateTime { get; set; }

    [Required(ErrorMessage = "Le statut du rendez-vous est requis.")]
    [MaxLength(20, ErrorMessage = "Le statut ne peut pas dépasser 20 caractères.")]
    public string Status { get; set; } // Exemple: "Available", "Scheduled", "Completed"

    [MaxLength(250, ErrorMessage = "Les notes ne peuvent pas dépasser 250 caractères.")]
    public string? Notes { get; set; } // Notes pour le rendez-vous
    public string? DocumentPath { get; set; } // Chemin du fichier PDF uploadé par le patient (si applicable)

}
