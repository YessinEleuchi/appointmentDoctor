using System.ComponentModel.DataAnnotations;

namespace AppointmentDoctor.DTO
{
    public class UpdateUserDTO
    {
        [EmailAddress(ErrorMessage = "L'adresse email n'est pas valide.")]
        public string? Email { get; set; }

        [Phone(ErrorMessage = "Le numéro de téléphone n'est pas valide.")]
        public string? PhoneNumber { get; set; }

        [MaxLength(200, ErrorMessage = "L'adresse ne peut pas dépasser 200 caractères.")]
        public string? Adress { get; set; }

        [MaxLength(50, ErrorMessage = "Le prénom ne peut pas dépasser 50 caractères.")]
        public string? FirstName { get; set; }

        [MaxLength(50, ErrorMessage = "Le nom de famille ne peut pas dépasser 50 caractères.")]
        public string? LastName { get; set; }

        [DataType(DataType.Password)]
        public string? CurrentPassword { get; set; }

        [DataType(DataType.Password)]
        [MinLength(8, ErrorMessage = "Le mot de passe doit contenir au moins 8 caractères.")]
        public string? NewPassword { get; set; }

        [Compare("NewPassword", ErrorMessage = "Les mots de passe ne correspondent pas.")]
        [DataType(DataType.Password)]
        public string? ConfirmPassword { get; set; }
    }
}
