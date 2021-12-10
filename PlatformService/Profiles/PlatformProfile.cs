using AutoMapper;
using PlatformService.Dtos;
using PlatformService.Models;

namespace PlatformService.PlatformProfile
{
    public class PlatformProfile: Profile
    {
        public PlatformProfile()
        {
            // Source -> Target
            // Learning-note: as both classes have properties that are named identically, don't have to do more work to get this working.
            CreateMap<Platform, PlatformReadDto>();

            // This was around as we get create Dto from user and we convert to platform for our app to use.
            CreateMap<PlatformCreateDto, Platform>();

            CreateMap<PlatformReadDto, PlatformPublishedDto>();

            CreateMap<Platform, GrpcPlatformModel>()
                .ForMember(dest => dest.PlatformId, opt => opt.MapFrom(src => src.Id)) // only this first one needed, the other ones done cause why not
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Publisher, opt => opt.MapFrom(src => src.Publisher));
        }
    }
}