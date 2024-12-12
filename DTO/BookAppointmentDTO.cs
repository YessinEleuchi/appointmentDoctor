using System.ComponentModel.DataAnnotations;

namespace AppointmentDoctor.DTO
{
    public class BookAppointmentDTO
    {
        [Required]
        public int AppointmentID { get; set; }
        [Required]
        public string Status { get; set; } 

        public string? PatientId { get; set; } 

       
    }
}

