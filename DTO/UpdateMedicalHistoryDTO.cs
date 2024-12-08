using System.ComponentModel.DataAnnotations;

namespace AppointmentDoctor.DTO
{
    public class UpdateMedicalHistoryDTO
    {
        [Required]
        public int MedicalHistoryID { get; set; }

        [MaxLength(255)]
        [Required]
        public string MedicalCondition { get; set; }
        [MaxLength(255)]
        public string Medications { get; set; }
        [MaxLength(255)]
        public string Allergies { get; set; }
        [MaxLength(255)]
        public string Surgeries { get; set; }
        [MaxLength(255)]
        public string FamilyMedicalHistory { get; set; }
    }
}
