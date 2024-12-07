namespace AppointmentDoctor.Models
{
    // Classe pour les Patients
    public class Patient : ApplicationUser
    {
        public string MedicalHistory { get; set; }  // Antécédents médicaux du patient
    }
}
