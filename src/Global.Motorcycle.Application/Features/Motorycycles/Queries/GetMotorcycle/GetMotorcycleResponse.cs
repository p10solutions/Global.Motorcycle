using Global.Motorcycle.Domain.Entities;

namespace Global.Motorcycle.Application.Features.Motorycycles.Queries.GetMotorcycle
{
    public class GetMotorcycleResponse(Guid id, DateTime createDate,
        DateTime? updateDate, EMotorcycleStatus status, string model, string plate, int year)
    {
        public Guid Id { get; init; } = id;
        public string Model { get; init; } = model;
        public string Plate { get; init; } = plate;
        public int Year { get; init; } = year;
        public DateTime CreateDate { get; init; } = createDate;
        public DateTime? UpdateDate { get; init; } = updateDate;
        public EMotorcycleStatus Status { get; init; } = status;
    }
}
