using System;
using System.ComponentModel.DataAnnotations;

namespace AppointmentDoctor.DTO
{
    public class CreateAppointmentDTO
    {
        public string? PatientId { get; set; }

        [Required]
        public DateTime AppointmentDateTime { get; set; }
    }
}
