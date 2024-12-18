using System.ComponentModel.DataAnnotations;

namespace AppointmentDoctor.DTO
{
    public class ForgotPasswordDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
