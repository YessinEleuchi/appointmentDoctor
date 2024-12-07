using System.ComponentModel.DataAnnotations;

public class RegisterAdminDTO
{
    [Required]
    public string Username { get; set; }

    [Required]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }

    [Required]
    public string Address { get; set; }
}
