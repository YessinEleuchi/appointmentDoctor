using System.ComponentModel.DataAnnotations;

namespace AppointmentDoctor.DTO
{
    public class RegisterDoctor
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }


        [Required]
        public string Adress { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string PhoneNumber { get; set; }


        [Required]
        public string Specialty { get; set; }
    }
}
