using System.ComponentModel.DataAnnotations;

namespace AppointmentDoctor.DTO
{
    public class BookAppointmentDTO
    {
        [Required(ErrorMessage = "L'ID du rendez-vous est requis.")]
        public int AppointmentID { get; set; }

        public string? PatientId { get; set; }
        public IFormFile? Document { get; set; } // Nouveau champ pour le fichier (PDF, JPG, JPEG, etc.)

    }
}
