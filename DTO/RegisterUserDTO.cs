using System.ComponentModel.DataAnnotations;

public class RegisterUserDTO
{
        [Required]
      [MaxLength(100)]
       public string Username { get; set; }
        [Required]
       [MaxLength(50)]
      public string FirstName { get; set; }

       [Required]
      [MaxLength(50)]
    public string LastName { get;set; }

      [Required]
      public string Adress { get; set; } 

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
    [Required]
    public string PhoneNumber { get; set; }

    public string Role { get; set; } = "patient"; // Rôle par défaut
    [Required]
    public string Gender { get; set; }
    [Required]
    public int Age { get; set; }






}
