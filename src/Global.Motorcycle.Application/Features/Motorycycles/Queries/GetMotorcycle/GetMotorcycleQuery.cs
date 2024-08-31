using Global.Motorcycle.Application.Features.Common;
using MediatR;

namespace Global.Motorcycle.Application.Features.Motorycycles.Queries.GetMotorcycle
{
    public class GetMotorcycleQuery(string plate) : CommandBase<GetMotorcycleQuery>(new GetMotorcycleQueryValidator()), 
        IRequest<IEnumerable<GetMotorcycleResponse>>
    {
        public string? Plate { get; init; } = plate;
    }
}
