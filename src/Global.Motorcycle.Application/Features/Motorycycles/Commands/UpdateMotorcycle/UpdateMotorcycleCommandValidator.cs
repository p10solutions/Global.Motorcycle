using FluentValidation;

namespace Global.Motorcycle.Application.Features.Motorcycles.Commands.UpdateMotorcycle
{
    public class UpdateMotorcycleCommandValidator : AbstractValidator<UpdateMotorcycleCommand>
    {
        public UpdateMotorcycleCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Model).NotEmpty();
            RuleFor(x => x.Model).Length(2, 200);
            RuleFor(x => x.Plate).NotEmpty();
            RuleFor(x => x.Plate).Length(2, 10);
            RuleFor(x => x.Year).NotEmpty();
            RuleFor(x => x.Status).NotEmpty();
        }
    }
}
