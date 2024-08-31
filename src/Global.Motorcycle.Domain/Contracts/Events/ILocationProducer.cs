using Global.Motorcycle.Domain.Models.Events.Locations;

namespace Global.Motorcycle.Domain.Contracts.Events
{
    public interface ILocationProducer
    {
        Task SendCreatedEventAsync(CreatedLocationEvent @event);
        Task SendReturnedLeaseEventAsync(ReturnedLeaseEvent @event);
    }
}
