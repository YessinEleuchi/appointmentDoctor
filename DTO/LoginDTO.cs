using System.ComponentModel.DataAnnotations;

namespace AppointmentDoctor.DTO
{
    public class LoginDTO
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        public string Password { get; set; }
    }
}