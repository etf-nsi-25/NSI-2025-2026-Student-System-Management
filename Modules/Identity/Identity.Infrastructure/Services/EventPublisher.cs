using Identity.Core.Repositories;
using Identity.Core.Events;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Logging; 

namespace Identity.Infrastructure.Services;


public class EventPublisher : IEventPublisher
{
    private readonly ILogger<EventPublisher> _logger; 

    public EventPublisher(ILogger<EventPublisher> logger)
    {
        _logger = logger;
    }

    public Task PublishAsync<TEvent>(TEvent domainEvent) where TEvent : class
    {
        _logger.LogInformation("Domain Event Published: {EventType} with data: {@EventData}", 
            typeof(TEvent).Name, domainEvent);
        
        
        return Task.CompletedTask;
    }
}