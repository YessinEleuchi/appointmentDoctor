using System.ComponentModel.DataAnnotations;

namespace AppointmentDoctor.DTO
{
    public class UserDTO
    {
        [MaxLength(50)]
        public string FirstName { get; set; }
        [MaxLength(50)]
        public string LastName { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }
        public string Gender { get; set; }
        public int Age { get; set; }





        public static UserDTO FromApplicationUser(ApplicationUser user)
        {
            return new UserDTO
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = user.UserName,
                Email = user.Email,
            };
        }
    }
}