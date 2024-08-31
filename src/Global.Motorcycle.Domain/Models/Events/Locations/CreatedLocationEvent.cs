using Global.Motorcycle.Domain.Entities;
using Global.Motorcycle.Domain.Models.Events.Plans;

namespace Global.Motorcycle.Domain.Models.Events.Locations
{
    public record CreatedLocationEvent
    {
        public Guid Id { get; set; }
        public Guid DeliverymanId { get; set; }
        public Guid PlanId { get; set; }
        public Guid MotorcycleId { get; set; }
        public DateTime InitialDate { get; set; }
        public DateTime EndDate { get; set; }
        public PlanEvent Plan { get; set; }
        ELocationStatus Status { get; set; }
    }
}
