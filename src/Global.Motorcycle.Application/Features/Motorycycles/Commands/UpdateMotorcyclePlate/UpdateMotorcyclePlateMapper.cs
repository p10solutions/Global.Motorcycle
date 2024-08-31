using AutoMapper;
using Global.Motorcycle.Domain.Entities;

namespace Global.Motorcycle.Application.Features.Motorycycles.Commands.UpdateMotorcyclePlate
{
    public class UpdateMotorcyclePlateMapper : Profile
    {
        public UpdateMotorcyclePlateMapper()
        {
            CreateMap<MotorcycleEntity, UpdateMotorcyclePlateResponse>();
        }
    }
}
