using AppointmentDoctor.DTO;
using AppointmentDoctor.Models;
using AutoMapper;

namespace AppointmentDoctor.Mapping
{
    public class MedicalHistoryMappingProfile : Profile
    {
        public MedicalHistoryMappingProfile()
        {
            // Map MedicalHistory to CreateMedicalHistoryDTO
            CreateMap<MedicalHistory, CreateMedicalHistoryDTO>()
                .ReverseMap(); // Reverse the map to allow bidirectional mapping
        }
    }
}
