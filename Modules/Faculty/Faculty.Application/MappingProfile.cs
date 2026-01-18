using AutoMapper;
using Faculty.Application.DTOs;
using Faculty.Core.Entities;

namespace Faculty.Application
{
    public class MappingProfile: Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateAssignmentDTO, Assignment>();
            CreateMap<Assignment, AssignmentDTO>().ForMember(
                dest => dest.Major,
                opt => opt.MapFrom(src => src.Course.Code)
            );
        }
    }
}
