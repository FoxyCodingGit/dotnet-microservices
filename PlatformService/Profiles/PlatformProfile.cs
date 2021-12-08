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
        }
    }
}