using AutoMapper;
using Global.Motorcycle.Domain.Entities;
using Global.Motorcycle.Domain.Models.Events.Locations;
using Global.Motorcycle.Domain.Models.Events.Plans;

namespace Global.Motorcycle.Application.Features.Locations.Commands.CreateLocation
{
    public class CreateLocationMapper: Profile
    {
        public CreateLocationMapper()
        {
            CreateMap<CreateLocationCommand, Location>();
            CreateMap<Location, CreateLocationResponse>();
            CreateMap<Location, CreatedLocationEvent>();
            CreateMap<Plan, PlanEvent>();
        }
    }
}
