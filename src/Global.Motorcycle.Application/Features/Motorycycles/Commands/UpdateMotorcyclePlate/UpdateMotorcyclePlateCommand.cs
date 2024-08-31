using Global.Motorcycle.Application.Features.Common;
using MediatR;

namespace Global.Motorcycle.Application.Features.Motorycycles.Commands.UpdateMotorcyclePlate
{
    public class UpdateMotorcyclePlateCommand(Guid id, string plate)
        : CommandBase<UpdateMotorcyclePlateCommand>(new UpdateMotorcyclePlateCommandValidator()), IRequest<UpdateMotorcyclePlateResponse>
    {
        public Guid Id { get; init; } = id;
        public string Plate { get; init; } = plate;
    }
}
