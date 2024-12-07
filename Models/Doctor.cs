namespace AppointmentDoctor.Models
{
    // Classe pour les Docteurs
    public class Doctor : ApplicationUser
    {
        public string Specialty { get; set; }  // Spécialité du docteur
    }
}
