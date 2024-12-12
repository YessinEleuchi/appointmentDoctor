using System.ComponentModel.DataAnnotations;

namespace AppointmentDoctor.DTO
{
    public class UpdateUserDTO
    {
        [EmailAddress(ErrorMessage = "L'adresse email n'est pas valide.")]
        public string? Email { get; set; } // Optionnel

        public string? PhoneNumber { get; set; } // Optionnel

        public string? Adress { get; set; } // Optionnel

        [MaxLength(50, ErrorMessage = "Le prénom ne peut pas dépasser 50 caractères.")]
        public string? FirstName { get; set; } // Optionnel

        [MaxLength(50, ErrorMessage = "Le nom de famille ne peut pas dépasser 50 caractères.")]
        public string? LastName { get; set; } // Optionnel

        [DataType(DataType.Password)]
        public string? CurrentPassword { get; set; } // Optionnel

        [DataType(DataType.Password)]
        public string? NewPassword { get; set; } // Optionnel

        [Compare("NewPassword", ErrorMessage = "Les mots de passe ne correspondent pas.")]
        [DataType(DataType.Password)]
        public string? ConfirmPassword { get; set; } // Optionnel
    }
}
