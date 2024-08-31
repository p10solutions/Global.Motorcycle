using FluentValidation;

namespace Global.Motorcycle.Application.Features.Locations.Commands.CreateLocation
{
    public class CreateLocationCommandValidator : AbstractValidator<CreateLocationCommand>
    {
        public CreateLocationCommandValidator()
        {
            RuleFor(x => x.DeliverymanId).NotEmpty();
            RuleFor(x => x.MotorcycleId).NotEmpty();
            RuleFor(x => x.PlanId).NotEmpty();
        }
    }
}
