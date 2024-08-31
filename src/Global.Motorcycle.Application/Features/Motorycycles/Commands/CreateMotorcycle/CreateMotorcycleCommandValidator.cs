using FluentValidation;

namespace Global.Motorcycle.Application.Features.Motorcycles.Commands.CreateMotorcycle
{
    public class CreateMotorcycleCommandValidator : AbstractValidator<CreateMotorcycleCommand>
    {
        public CreateMotorcycleCommandValidator()
        {
            RuleFor(x => x.Model).NotEmpty();
            RuleFor(x => x.Model).Length(2, 200);
            RuleFor(x => x.Plate).NotEmpty();
            RuleFor(x => x.Plate).Length(2, 10);
            RuleFor(x => x.Year).NotEmpty();
            RuleFor(x => x.Status).NotEmpty();
        }
    }
}
