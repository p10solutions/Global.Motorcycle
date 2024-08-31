using Global.Motorcycle.Domain.Entities;

namespace Global.Motorcycle.Application.Features.Locations.Commands.CreateLocation
{
    public class CreateLocationResponse
    {
        public Guid Id { get; set; }
        public Guid DeliverymanId { get; set; }
        public Guid PlanId { get; set; }
        public Guid MotorcycleId { get; set; }
        public DateTime InitialDate { get;  set; }
        public DateTime EndDate { get;  set; }
        public ELocationStatus Status { get; set; }

        public CreateLocationResponse(Guid id, Guid deliverymanId, Guid planId, Guid motorcycleId, DateTime initialDate, DateTime endDate, ELocationStatus status)
        {
            Id = id;
            DeliverymanId = deliverymanId;
            PlanId = planId;
            MotorcycleId = motorcycleId;
            InitialDate = initialDate;
            EndDate = endDate;
            Status = status;
        }
    }
}
