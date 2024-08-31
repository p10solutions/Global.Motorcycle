using FluentValidation;

namespace Global.Motorcycle.Application.Features.Motorycycles.Queries.GetMotorcycleById
{
    public class GetMotorcycleByIdQueryValidator : AbstractValidator<GetMotorcycleByIdQuery>
    {
        public GetMotorcycleByIdQueryValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}
