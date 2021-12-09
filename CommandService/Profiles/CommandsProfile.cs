using AutoMapper;
using CommandService.Dtos;
using CommandService.Models;

namespace CommandsService.Profiles
{
    public class CommandsProfile: Profile
    {
        public CommandsProfile()
        {
            // Source -> Target
            CreateMap<Platform, PlatformReadDto>();
            CreateMap<CommandCreateDto, Command>();
            CreateMap<Command, CommandReadDto>();

            // We want the id of the source (platformPublishDto) to be mapped to the external id of the destination (Platform)
            // PlatformPublishDto.Id -> set to -> Platform.ExternalId
            CreateMap<PlatformPublishDto, Platform>()
                .ForMember(dest => dest.ExternalId, opt => opt.MapFrom(src => src.Id));
        }
    }
}