using System.ComponentModel.DataAnnotations;

public class Appointment
{
    public int AppointmentID { get; set; }

    [Required]
    public string DoctorId { get; set; }
    public ApplicationUser Doctor { get; set; }  // Médecin (relation de navigation)

    public string? PatientId { get; set; }
    public ApplicationUser? Patient { get; set; }  // Patient (relation de navigation)

    [Required]
    public DateTime AppointmentDateTime { get; set; }

    [Required]
    [MaxLength(20)]
    public string Status { get; set; } // Par exemple: "Available", "Scheduled", "Completed"

    [MaxLength(250)]
    public string? Notes { get; set; }  // Notes pour l'examen
}
