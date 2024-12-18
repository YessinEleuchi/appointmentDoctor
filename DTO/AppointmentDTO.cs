using System.ComponentModel.DataAnnotations;

namespace AppointmentDoctor.DTO
{
    public class AppointmentDTO
    {
        public int AppointmentID { get; set; }

        [Required(ErrorMessage = "L'ID du docteur est requis.")]
        public string DoctorId { get; set; }

        public string? PatientId { get; set; }

        [Required(ErrorMessage = "La date et l'heure du rendez-vous sont requises.")]
        public DateTime AppointmentDateTime { get; set; }

        [Required(ErrorMessage = "Le statut du rendez-vous est requis.")]
        [MaxLength(20, ErrorMessage = "Le statut ne peut pas dépasser 20 caractères.")]
        public string Status { get; set; }

        [MaxLength(250, ErrorMessage = "Les notes ne peuvent pas dépasser 250 caractères.")]
        public string? Notes { get; set; }

        public string DoctorName { get; set; } // Nom complet du docteur

        public string? PatientName { get; set; } // Nom complet du patient, peut être nul

        public string Speciality { get; set; } // Spécialité du docteur

        public string? DocumentPath { get; set; } // Path for the document

        // Méthode de mappage de l'entité Appointment vers AppointmentDTO
        public static AppointmentDTO FromAppointment(Appointment appointment)
        {
            return new AppointmentDTO
            {
                AppointmentID = appointment.AppointmentID,
                AppointmentDateTime = appointment.AppointmentDateTime,
                Status = appointment.Status,
                Notes = appointment.Notes,
                DoctorId = appointment.DoctorId,
                PatientId = appointment.PatientId,
                DoctorName = $"{appointment.Doctor?.FirstName} {appointment.Doctor?.LastName}",
                PatientName = appointment.Patient != null ? $"{appointment.Patient.FirstName} {appointment.Patient.LastName}" : null,
                Speciality = appointment.Doctor?.Speciality ?? "N/A",
                DocumentPath = appointment.DocumentPath // Ajout du document si disponible
            };
        }
    }
}
