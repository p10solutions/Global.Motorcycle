using Global.Motorcycle.Domain.Models.Events.Motorcycles;

namespace Global.Motorcycle.Domain.Contracts.Events
{
    public interface IMotorcycleProducer
    {
        Task SendCreatedEventAsync(CreatedMotorcycleEvent @event);
        Task SendUpdatedEventAsync(UpdatedMotorcycleEvent @event);
        Task SendDeletedEventAsync(DeletedMotorcycleEvent @event);
    }
}
