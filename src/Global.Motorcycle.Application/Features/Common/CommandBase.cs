using FluentValidation;
using Global.Motorcycle.Domain.Contracts.Validation;

namespace Global.Motorcycle.Application.Features.Common
{
    public class CommandBase<T>(AbstractValidator<T> validator) : IValidableEntity where T : CommandBase<T>
    {
        IEnumerable<string> _erros = [];
        readonly AbstractValidator<T> _validator = validator;
        public ISet<string> Errors => new HashSet<string>(_erros);

        public bool Validate()
        {
            var result = _validator.Validate((T)this);
            _erros = result.Errors.Select(x => x.ErrorMessage);

            return result.IsValid;
        }
    }
}
