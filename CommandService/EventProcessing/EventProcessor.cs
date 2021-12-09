using System;
using System.Text.Json;
using AutoMapper;
using CommandService.Data;
using CommandService.Dtos;
using CommandService.Models;
using Microsoft.Extensions.DependencyInjection;

namespace CommandService.EventProcesing
{
    public class EventProcessor : IEventProcessor
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private IMapper _mapper;

        public EventProcessor(IServiceScopeFactory scopeFactory, IMapper mapper)
        {
            _scopeFactory = scopeFactory;
            _mapper = mapper;
        }

        public void ProcessEvent(string message)
        {
            var eventType = DetermineEvent(message);

            switch (eventType)
            {
                case EventType.PlatformPublished:
                    addPlatform(message);
                    break;
                default:
                    break;
            }
        }

        private EventType DetermineEvent(string notificationMessage)
        {
            Console.WriteLine("--> Determining Event");

            var eventType =  JsonSerializer.Deserialize<GenericEventDto>(notificationMessage); // When we deserialise it, we only want the event property so thats why we have GenericEventDto.
            
            switch(eventType.Event)
            {
                case "Platform_Published":
                    Console.WriteLine("--> Platform Published event detected");
                    return EventType.PlatformPublished;
                default:
                    Console.WriteLine("--> Could not determine the event type");
                    return EventType.Undetermined;
            }
        }

        private void addPlatform(string platformPublishedMessage)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var repo = scope.ServiceProvider.GetRequiredService<ICommandRepo>(); // This is done instead of dependency injection because of the service lifetime of the repo.

                var platformPublishedDto = JsonSerializer.Deserialize<PlatformPublishDto>(platformPublishedMessage);

                try
                {
                    var plat = _mapper.Map<Platform>(platformPublishedDto);
                    if (!repo.ExternalPlatformExist(plat.ExternalId))
                    {
                        repo.CreatePlatform(plat);
                        repo.SaveChanges();
                        Console.WriteLine("--> Platform added!");
                    }
                    else
                    {
                        Console.WriteLine("--> Platform already exists...");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"--> Could not add platform to DB {e.Message}");
                }
            }
        }
    }

    enum EventType
    {
        PlatformPublished,
        Undetermined
    }
}