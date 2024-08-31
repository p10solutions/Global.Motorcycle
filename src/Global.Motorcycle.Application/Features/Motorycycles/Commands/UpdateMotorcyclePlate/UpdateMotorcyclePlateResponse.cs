using Global.Motorcycle.Domain.Entities;

namespace Global.Motorcycle.Application.Features.Motorycycles.Commands.UpdateMotorcyclePlate
{
    public class UpdateMotorcyclePlateResponse(Guid id, string plate, string model, int year, DateTime createDate, EMotorcycleStatus status)
    {
        public Guid Id { get; init; } = id;
        public string Model { get; init; } = model;
        public string Plate { get; init; } = plate;
        public int Year { get; init; } = year;
        public DateTime CreateDate { get; init; } = createDate;
        public EMotorcycleStatus Status { get; init; } = status;
    }
}
