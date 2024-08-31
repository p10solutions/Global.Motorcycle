using FluentValidation;

namespace Global.Motorcycle.Application.Features.Locations.Commands.ReturnLease
{
    public class ReturnLeaseCommandValidator : AbstractValidator<ReturnLeaseCommand>
    {
        public ReturnLeaseCommandValidator()
        {
            RuleFor(x => x.LocationId).NotEmpty();
            RuleFor(x => x.DateTime)
                .NotEmpty()
                .Must((x) => x <= DateTime.Now).WithMessage("Date cannot be greater than the current date");
        }
    }
}
