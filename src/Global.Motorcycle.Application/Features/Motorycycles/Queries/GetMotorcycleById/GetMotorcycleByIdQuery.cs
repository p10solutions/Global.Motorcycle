using Global.Motorcycle.Application.Features.Common;
using MediatR;

namespace Global.Motorcycle.Application.Features.Motorycycles.Queries.GetMotorcycleById
{
    public class GetMotorcycleByIdQuery : CommandBase<GetMotorcycleByIdQuery>, IRequest<GetMotorcycleByIdResponse>
    {
        public GetMotorcycleByIdQuery(Guid id) : base(new GetMotorcycleByIdQueryValidator())
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }
}
