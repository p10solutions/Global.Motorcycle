using FluentValidation;

namespace Global.Motorcycle.Application.Features.Motorycycles.Commands.DeleteMotorcycle
{
    public class DeleteMotorcycleCommandValidator : AbstractValidator<DeleteMotorcycleCommand>
    {
        public DeleteMotorcycleCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}
