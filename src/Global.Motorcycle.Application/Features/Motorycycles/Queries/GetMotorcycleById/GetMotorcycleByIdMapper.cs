using AutoMapper;
using Global.Motorcycle.Domain.Entities;

namespace Global.Motorcycle.Application.Features.Motorycycles.Queries.GetMotorcycleById
{
    public class GetMotorcycleByIdMapper : Profile
    {
        public GetMotorcycleByIdMapper()
        {
            CreateMap<MotorcycleEntity, GetMotorcycleByIdResponse>();
        }
    }
}
