using System.ComponentModel.DataAnnotations;

namespace AppointmentDoctor.DTO
{
    public class UserDTO
    {
        [MaxLength(50, ErrorMessage = "Le prénom ne peut pas dépasser 50 caractères.")]
        public string FirstName { get; set; }

        [MaxLength(50, ErrorMessage = "Le nom de famille ne peut pas dépasser 50 caractères.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Le nom d'utilisateur est requis.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "L'adresse email est requise.")]
        [EmailAddress(ErrorMessage = "L'adresse email n'est pas valide.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Le genre est requis.")]
        [RegularExpression("^(Male|Female|Other)$", ErrorMessage = "Genre invalide.")]
        public string Gender { get; set; }

        [Required(ErrorMessage = "L'âge est requis.")]
        [Range(0, 120, ErrorMessage = "L'âge doit être compris entre 0 et 120 ans.")]
        public int Age { get; set; }

        public static UserDTO FromApplicationUser(ApplicationUser user)
        {
            return new UserDTO
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = user.UserName,
                Email = user.Email,
                Gender = user.Gender,
                Age = user.Age
            };
        }
    }
}
