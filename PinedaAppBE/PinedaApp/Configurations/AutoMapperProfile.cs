using PinedaApp.Models.DTO;
using PinedaApp.Models;
using AutoMapper;

namespace PinedaApp.Configurations
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Academic, AcademicDto>();
            CreateMap<Portfolio, PortfolioDto>();
            CreateMap<Experience, ExperienceDto>()
                .ForMember(dest => dest.Projects, opt => opt.MapFrom(src => src.ProjectHandled));
            CreateMap<ProjectHandled, ProjectHandledDto>();
        }
    }
}
