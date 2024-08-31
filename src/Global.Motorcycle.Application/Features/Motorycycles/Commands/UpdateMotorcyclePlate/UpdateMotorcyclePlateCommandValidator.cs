using FluentValidation;

namespace Global.Motorcycle.Application.Features.Motorycycles.Commands.UpdateMotorcyclePlate
{
    public class UpdateMotorcyclePlateCommandValidator : AbstractValidator<UpdateMotorcyclePlateCommand>
    {
        public UpdateMotorcyclePlateCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Plate).NotEmpty();
            RuleFor(x => x.Plate).Length(2, 10);
        }
    }
}
