using System.ComponentModel.DataAnnotations;

namespace AppointmentDoctor.DTO
{
    public class RegisterDoctor
    {
        [Required]
        public string Username { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Specialty { get; set; }
    }
}
