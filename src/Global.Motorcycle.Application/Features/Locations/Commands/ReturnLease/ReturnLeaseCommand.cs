using Global.Motorcycle.Application.Features.Common;
using MediatR;

namespace Global.Motorcycle.Application.Features.Locations.Commands.ReturnLease
{
    public class ReturnLeaseCommand(Guid locationId, DateTime dateTime)
                : CommandBase<ReturnLeaseCommand>(new ReturnLeaseCommandValidator()), IRequest<ReturnLeaseResponse>
    {
        public Guid LocationId { get; set; } = locationId;
        public DateTime DateTime { get; set; } = dateTime;
    }
}
