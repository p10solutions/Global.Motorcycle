namespace Global.Motorcycle.Domain.Models.Events.Motorcycles
{
    public record DeletedMotorcycleEvent
    {
        public DeletedMotorcycleEvent(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }
}
