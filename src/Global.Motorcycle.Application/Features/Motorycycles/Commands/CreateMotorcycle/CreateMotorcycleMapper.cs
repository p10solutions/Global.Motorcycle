using AutoMapper;
using Global.Motorcycle.Domain.Entities;
using Global.Motorcycle.Domain.Models.Events.Motorcycles;

namespace Global.Motorcycle.Application.Features.Motorcycles.Commands.CreateMotorcycle
{
    public class CreateMotorcycleMapper: Profile
    {
        public CreateMotorcycleMapper()
        {
            CreateMap<CreateMotorcycleCommand, MotorcycleEntity>();
            CreateMap<MotorcycleEntity, CreateMotorcycleResponse>();
            CreateMap<MotorcycleEntity, CreatedMotorcycleEvent>();
        }
    }
}
