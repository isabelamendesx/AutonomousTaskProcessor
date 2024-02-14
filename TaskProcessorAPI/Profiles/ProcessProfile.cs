using AutoMapper;
using AutonomousTaskProcessor.Entities;
using TaskProcessorAPI.Dtos;


namespace TaskProcessorAPI.Profiles
{
    public class ProcessProfile : Profile
    {
        public ProcessProfile()
        {
            CreateMap<Process, ReadProcessesDto>()
                .ForMember(dest => dest.SubprocessCount, opt => opt.MapFrom(src => src.SubProcesses.Count));

            CreateMap<Process, ReadProcessDto>();
        }
    }
}
