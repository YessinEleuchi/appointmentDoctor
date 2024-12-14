using AppointmentDoctor.DTO;
using AppointmentDoctor.Models;
using AutoMapper;

namespace AppointmentDoctor.Mapping
{
    public class AppointmentMappingProfile : Profile
    {
        public AppointmentMappingProfile()
        {
            // Map Appointment -> AppointmentDTO
            CreateMap<Appointment, AppointmentDTO>()
                .ForMember(dest => dest.DoctorName, opt => opt.MapFrom(src => $"{src.Doctor.FirstName} {src.Doctor.LastName}"))
                .ForMember(dest => dest.Speciality, opt => opt.Ignore()); // Adjust if needed

            // Map AppointmentDTO -> Appointment (if necessary)
            CreateMap<AppointmentDTO, Appointment>();
        }
    }
}
