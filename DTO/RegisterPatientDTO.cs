using System.ComponentModel.DataAnnotations;

public class RegisterPatientDTO
{
    [Required]
    public string Username { get; set; }
    [Required]
    public string Email { get; set; }
    [Required]
    public string Password { get; set; }
    [Required]
    public string Address { get; set; }
    public string MedicalHistory { get; set; } // Optionnel, antécédents médicaux pour le patient
}
