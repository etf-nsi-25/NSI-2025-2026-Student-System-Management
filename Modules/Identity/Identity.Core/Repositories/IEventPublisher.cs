using System.Threading.Tasks;

namespace Identity.Core.Repositories;

public interface IEventPublisher
{
    Task PublishAsync<TEvent>(TEvent domainEvent) where TEvent : class;
}