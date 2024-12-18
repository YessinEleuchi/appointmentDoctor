using System.ComponentModel.DataAnnotations;

public class RegisterUserDTO
{
    [Required(ErrorMessage = "Le nom d'utilisateur est requis.")]
    [MaxLength(100, ErrorMessage = "Le nom d'utilisateur ne peut pas dépasser 100 caractères.")]
    public string Username { get; set; }

    [Required(ErrorMessage = "Le prénom est requis.")]
    [MaxLength(50, ErrorMessage = "Le prénom ne peut pas dépasser 50 caractères.")]
    public string FirstName { get; set; }

    [Required(ErrorMessage = "Le nom de famille est requis.")]
    [MaxLength(50, ErrorMessage = "Le nom de famille ne peut pas dépasser 50 caractères.")]
    public string LastName { get; set; }

    [Required(ErrorMessage = "L'adresse est requise.")]
    [MaxLength(200, ErrorMessage = "L'adresse ne peut pas dépasser 200 caractères.")]
    public string Adress { get; set; }

    [Required(ErrorMessage = "Le mot de passe est requis.")]
    [DataType(DataType.Password)]
    [MinLength(8, ErrorMessage = "Le mot de passe doit contenir au moins 8 caractères.")]
    public string Password { get; set; }

    [Required(ErrorMessage = "L'adresse email est requise.")]
    [EmailAddress(ErrorMessage = "L'adresse email n'est pas valide.")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Le numéro de téléphone est requis.")]
    [Phone(ErrorMessage = "Le numéro de téléphone n'est pas valide.")]
    public string PhoneNumber { get; set; }

    [Required(ErrorMessage = "Le genre est requis.")]
    [RegularExpression("^(Male|Female|Other)$", ErrorMessage = "Genre invalide.")]
    public string Gender { get; set; }

    [Required(ErrorMessage = "L'âge est requis.")]
    [Range(0, 120, ErrorMessage = "L'âge doit être compris entre 0 et 120 ans.")]
    public int Age { get; set; }

    public string Role { get; set; } = "patient"; // Par défaut
}
