using System.ComponentModel.DataAnnotations;

namespace AppointmentDoctor.DTO
{
    public class CreateAppointmentDTO
    {
        public string? PatientId { get; set; } // Optionnel au moment de la création

        [Required(ErrorMessage = "La date et l'heure du rendez-vous sont requises.")]
        public DateTime AppointmentDateTime { get; set; }
    }
}
