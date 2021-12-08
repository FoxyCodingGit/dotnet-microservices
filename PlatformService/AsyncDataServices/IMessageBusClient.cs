using PlatformService.Dtos;

namespace PlatformService.AsyncDataServices
{
    public interface IMessageBusClient // Learning-note - remember that when you add an interface, to set it up for dependency injection with a concreate class in the startup.cs file.
    {
        void PublishNewPlatform(PlatformPublishedDto platformPublishedDto);
    }
}