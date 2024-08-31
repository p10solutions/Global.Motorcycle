using Global.Motorcycle.Application.Features.Common;
using MediatR;

namespace Global.Motorcycle.Application.Features.Motorycycles.Commands.DeleteMotorcycle
{
    public class DeleteMotorcycleCommand(Guid id)
        : CommandBase<DeleteMotorcycleCommand>(new DeleteMotorcycleCommandValidator()), IRequest<DeleteMotorcycleResponse>
    {
        public Guid Id { get; init; } = id;
    }
}
