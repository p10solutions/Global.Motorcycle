namespace Global.Motorcycle.Application.Features.Motorycycles.Commands.DeleteMotorcycle
{
    public class DeleteMotorcycleResponse(Guid id)
    {
        public Guid Id { get; init; } = id;
    }
}
