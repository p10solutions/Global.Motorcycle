using AutoMapper;
using Global.Motorcycle.Domain.Entities;
using Global.Motorcycle.Domain.Models.Events.Locations;
using Global.Motorcycle.Domain.Models.Events.Plans;

namespace Global.Motorcycle.Application.Features.Locations.Commands.ReturnLease
{
    public class ReturnLeaseMapper: Profile
    {
        public ReturnLeaseMapper()
        {
            CreateMap<Location, ReturnLeaseResponse>();
            CreateMap<Location, ReturnedLeaseEvent>();
            CreateMap<Plan, PlanEvent>();
        }
    }
}
