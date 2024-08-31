using Global.Motorcycle.Api.Controllers.Base;
using Global.Motorcycle.Application.Features.Motorcycles.Commands.CreateMotorcycle;
using Global.Motorcycle.Application.Features.Motorcycles.Commands.UpdateMotorcycle;
using Global.Motorcycle.Application.Features.Motorycycles.Commands.DeleteMotorcycle;
using Global.Motorcycle.Application.Features.Motorycycles.Commands.UpdateMotorcyclePlate;
using Global.Motorcycle.Application.Features.Motorycycles.Queries.GetMotorcycle;
using Global.Motorcycle.Application.Features.Motorycycles.Queries.GetMotorcycleById;
using Global.Motorcycle.Domain.Contracts.Notifications;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Global.Motorcycle.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MotorcycleController(IMediator mediator, INotificationsHandler notifications) : ApiControllerBase(mediator, notifications)
    {
        [HttpGet("id")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetMotorcycleByIdResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByIdAsync(Guid id)
            => await SendAsync(new GetMotorcycleByIdQuery(id));

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<GetMotorcycleResponse>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAsync([FromQuery]string? plate)
            => await SendAsync(new GetMotorcycleQuery(plate));

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CreateMotorcycleResponse))]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostAsync(CreateMotorcycleCommand createMotorcycleCommand)
            => await SendAsync(createMotorcycleCommand, HttpStatusCode.Created);

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UpdateMotorcycleResponse))]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PutAsync(Guid id, UpdateMotorcycleCommand updateMotorcycleCommand)
            => await SendAsync(updateMotorcycleCommand);

        [HttpPatch("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UpdateMotorcycleResponse))]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PatchAsync(Guid id, UpdateMotorcyclePlateCommand updateMotorcyclePlateCommand)
            => await SendAsync(updateMotorcyclePlateCommand);

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAsync(Guid id)
            => await SendAsync(new DeleteMotorcycleCommand(id));
    }
}
