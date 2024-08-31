using Global.Motorcycle.Application.Features.Common;
using Global.Motorcycle.Domain.Entities;
using MediatR;

namespace Global.Motorcycle.Application.Features.Motorcycles.Commands.UpdateMotorcycle
{
    public class UpdateMotorcycleCommand(Guid id, string model, string plate, int year, EMotorcycleStatus status)
        : CommandBase<UpdateMotorcycleCommand>(new UpdateMotorcycleCommandValidator()), IRequest<UpdateMotorcycleResponse>
    {
        public Guid Id { get; init; } = id;
        public string Model { get; init; } = model;
        public string Plate { get; init; } = plate;
        public int Year { get; init; } = year;
        public EMotorcycleStatus Status { get; init; } = status;
    }
}
