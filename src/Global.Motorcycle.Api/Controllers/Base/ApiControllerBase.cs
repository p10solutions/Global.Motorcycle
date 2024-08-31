using Global.Motorcycle.Domain.Contracts.Notifications;
using Global.Motorcycle.Domain.Models.Notifications;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Global.Motorcycle.Api.Controllers.Base
{
    public class ApiControllerBase : ControllerBase
    {
        protected readonly IMediator _mediator;
        protected readonly INotificationsHandler _notifications;

        public ApiControllerBase(IMediator mediator, INotificationsHandler notifications)
        {
            _mediator = mediator;
            _notifications = notifications;
        }

        protected async Task<IActionResult> SendAsync<TOutput>(IRequest<TOutput> request, HttpStatusCode successStatusCode = HttpStatusCode.OK)
            => Send(await _mediator.Send(request), successStatusCode);

        IActionResult Send(object response, HttpStatusCode successStatusCode)
        {
            if (_notifications.HasErrors())
                return GetResult(_notifications.Erros);

            return new ObjectResult(response) { StatusCode = (int)successStatusCode };
        }

        ObjectResult GetResult(object result)
        {
            return _notifications.GetNotificationType() switch
            {
                ENotificationType.BusinessValidation => new UnprocessableEntityObjectResult(result),
                ENotificationType.Unauthorized => new UnauthorizedObjectResult(result),
                ENotificationType.NotFound => new NotFoundObjectResult(result),
                _ => new ObjectResult(result) { StatusCode = (int)HttpStatusCode.InternalServerError },
            };
        }
    }
}
