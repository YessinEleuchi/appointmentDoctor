using AppointmentDoctor.DTO;
using AppointmentDoctor.Models;
using AutoMapper;

namespace AppointmentDoctor.Mapping
{
    public class AppointmentMappingProfile : Profile
    {
        public AppointmentMappingProfile()
        {
            CreateMap<Appointment, AppointmentDTO>()
    .ForMember(dest => dest.DoctorName, opt => opt.MapFrom(src => $"{src.Doctor.FirstName} {src.Doctor.LastName}"))
    .ForMember(dest => dest.Speciality, opt => opt.MapFrom(src => src.Doctor.Speciality))
    .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => $"{src.Patient.FirstName} {src.Patient.LastName}"));


            // Map AppointmentDTO -> Appointment (if necessary)
            CreateMap<AppointmentDTO, Appointment>();
        }
    }
}
