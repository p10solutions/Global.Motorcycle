using Global.Motorcycle.Domain.Entities;

namespace Global.Motorcycle.Application.Features.Motorycycles.Queries.GetMotorcycleById
{
    public class GetMotorcycleByIdResponse
    {
        public Guid Id { get; init; }
        public string Model { get; init; }
        public string Plate { get; init; }
        public int Year { get; init; }
        public DateTime CreateDate { get; init; }
        public DateTime? UpdateDate { get; init; }
        public EMotorcycleStatus Status { get; init; }

        public GetMotorcycleByIdResponse(Guid id, DateTime createDate,
            DateTime? updateDate, EMotorcycleStatus status, string model, string plate, int year)
        {
            Id = id;
            Model = model;
            Plate = plate;
            Year = year;
            CreateDate = createDate;
            UpdateDate = updateDate;
            Status = status;
        }
    }
}
