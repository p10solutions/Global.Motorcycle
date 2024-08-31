using AutoMapper;
using Global.Motorcycle.Domain.Entities;

namespace Global.Motorcycle.Application.Features.Motorycycles.Queries.GetMotorcycle
{
    public class GetMotorcycleMapper : Profile
    {
        public GetMotorcycleMapper()
        {
            CreateMap<MotorcycleEntity, GetMotorcycleResponse>();
        }
    }
}
