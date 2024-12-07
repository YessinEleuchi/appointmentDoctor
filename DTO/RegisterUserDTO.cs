using System.ComponentModel.DataAnnotations;

namespace AppointmentDoctor.DTO
{
    public class RegisterUserDTO
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string Role { get; set; } = "patient"; // Rôle par défaut
        public string? Address { get; set; } // Adresse pour tous les utilisateurs
        public string? Specialty { get; set; } // Spécialité pour les docteurs
    }
}
