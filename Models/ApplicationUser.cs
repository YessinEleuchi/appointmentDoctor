using Microsoft.AspNetCore.Identity;

namespace AppointmentDoctor.Models
{
    // ApplicationUser - Classe de base
    public class ApplicationUser : IdentityUser
    {
        public string Address { get; set; }  // Adresse commune à tous les utilisateurs

        // Vous pouvez ajouter ici d'autres attributs communs si nécessaire
    }
}
