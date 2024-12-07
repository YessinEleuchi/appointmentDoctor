using System.ComponentModel.DataAnnotations;

namespace AppointmentDoctor.Models
{
    public class Specialty
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
    }
}
