using AutoMapper;
using Global.Motorcycle.Domain.Entities;
using Global.Motorcycle.Domain.Models.Events.Motorcycles;

namespace Global.Motorcycle.Application.Features.Motorcycles.Commands.UpdateMotorcycle
{
    public class UpdateMotorcycleMapper : Profile
    {
        public UpdateMotorcycleMapper()
        {
            CreateMap<UpdateMotorcycleCommand, MotorcycleEntity>()
                .ForMember(dest => dest.UpdateDate, opt => opt.MapFrom(src=> DateTime.Now));
            CreateMap<MotorcycleEntity, UpdateMotorcycleResponse>();
            CreateMap<MotorcycleEntity, UpdatedMotorcycleEvent>();
        }
    }
}
