using Microsoft.AspNetCore.Identity;

namespace AppointmentDoctor.Models
{
    public class ApplicationUser : IdentityUser
    {
        public int? SpecialtyId { get; set; } // Specialty Id (nullable for non-doctors)
        public Specialty Specialty { get; set; } // Navigation property
    }
}
