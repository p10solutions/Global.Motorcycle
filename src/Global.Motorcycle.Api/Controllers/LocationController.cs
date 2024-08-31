using Global.Motorcycle.Api.Controllers.Base;
using Global.Motorcycle.Application.Features.Locations.Commands.CreateLocation;
using Global.Motorcycle.Application.Features.Locations.Commands.ReturnLease;
using Global.Motorcycle.Application.Features.Motorcycles.Commands.CreateMotorcycle;
using Global.Motorcycle.Application.Features.Motorcycles.Commands.UpdateMotorcycle;
using Global.Motorcycle.Domain.Contracts.Notifications;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Global.Motorcycle.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationController(IMediator mediator, INotificationsHandler notifications) : ApiControllerBase(mediator, notifications)
    {
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CreateMotorcycleResponse))]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PostAsync(CreateLocationCommand createLocationCommand)
            => await SendAsync(createLocationCommand, HttpStatusCode.Created);


        [HttpPatch("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UpdateMotorcycleResponse))]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PatchAsync(Guid id, ReturnLeaseCommand returnLeaseCommand)
            => await SendAsync(returnLeaseCommand);
    }
}
