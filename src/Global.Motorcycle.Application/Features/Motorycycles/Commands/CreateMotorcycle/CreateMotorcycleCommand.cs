using Global.Motorcycle.Application.Features.Common;
using Global.Motorcycle.Domain.Entities;
using MediatR;

namespace Global.Motorcycle.Application.Features.Motorcycles.Commands.CreateMotorcycle
{
    public class CreateMotorcycleCommand(string model, string plate, int year, EMotorcycleStatus status) 
        : CommandBase<CreateMotorcycleCommand>(new CreateMotorcycleCommandValidator()), IRequest<CreateMotorcycleResponse>
    {
        public string Model { get; init; } = model;
        public string Plate { get; init; } = plate;
        public int Year { get; init; } = year;
        public EMotorcycleStatus Status { get; init; } = status;
    }
}
