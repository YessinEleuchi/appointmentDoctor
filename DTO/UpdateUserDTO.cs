namespace AppointmentDoctor.DTO
{
    public class UpdateUserDTO
    {
        public string? NewUsername { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; } // Mise à jour de l'adresse
        public string? Specialty { get; set; } // Mise à jour de la spécialité
    }
}
