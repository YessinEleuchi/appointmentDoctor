using System.ComponentModel.DataAnnotations;

public class RegisterDoctorDTO
{
    [Required]
    public string Username { get; set; }
    [Required]
    public string Email { get; set; }
    [Required]
    public string Password { get; set; }
    [Required]
    public string Address { get; set; }
    [Required]
    public string Specialty { get; set; }  // Spécialité obligatoire pour les docteurs
}
