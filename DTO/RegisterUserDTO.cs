using System.ComponentModel.DataAnnotations;

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
  
}
