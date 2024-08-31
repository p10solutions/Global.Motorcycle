using Global.Motorcycle.Application.Features.Common;
using MediatR;

namespace Global.Motorcycle.Application.Features.Locations.Commands.CreateLocation
{
    public class CreateLocationCommand(Guid deliverymanId, Guid planId, Guid motorcycleId)
                : CommandBase<CreateLocationCommand>(new CreateLocationCommandValidator()), IRequest<CreateLocationResponse>
    {
        public Guid DeliverymanId { get; set; } = deliverymanId;
        public Guid PlanId { get; set; } = planId;
        public Guid MotorcycleId { get; set; } = motorcycleId;
    }
}
