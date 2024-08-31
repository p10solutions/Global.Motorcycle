using AutoMapper;
using Global.Motorcycle.Domain.Contracts.Data;
using Global.Motorcycle.Domain.Contracts.Data.Repositories;
using Global.Motorcycle.Domain.Contracts.Events;
using Global.Motorcycle.Domain.Contracts.Notifications;
using Global.Motorcycle.Domain.Entities;
using Global.Motorcycle.Domain.Models.Events.Locations;
using Global.Motorcycle.Domain.Models.Notifications;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Global.Motorcycle.Application.Features.Locations.Commands.ReturnLease
{
    public class ReturnLeaseHandler : IRequestHandler<ReturnLeaseCommand, ReturnLeaseResponse>
    {
        readonly IMotorcycleRepository _motorcycleRepository;
        readonly ILogger<ReturnLeaseHandler> _logger;
        readonly IMapper _mapper;
        readonly INotificationsHandler _notificationsHandler;
        readonly IUnitOfWork _unitOfWork;
        readonly ILocationProducer _MotorcycleProducer;

        public ReturnLeaseHandler(IMotorcycleRepository motorcycleRepository, ILogger<ReturnLeaseHandler> logger,
            IMapper mapper, INotificationsHandler notificationsHandler, IUnitOfWork unitOfWork, ILocationProducer motorcycleProducer)
        {
            _motorcycleRepository = motorcycleRepository;
            _logger = logger;
            _mapper = mapper;
            _notificationsHandler = notificationsHandler;
            _unitOfWork = unitOfWork;
            _MotorcycleProducer = motorcycleProducer;
        }

        public async Task<ReturnLeaseResponse> Handle(ReturnLeaseCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var location = await _motorcycleRepository.GetLocationAsync(request.LocationId);

                if (location is null)
                {
                    _logger.LogWarning("The location was not found {LocationId}", request.LocationId);

                    return _notificationsHandler
                        .AddNotification("The location was not found", ENotificationType.NotFound)
                        .ReturnDefault<ReturnLeaseResponse>();
                }

                if (location.Status != ELocationStatus.Active)
                {
                    _logger.LogWarning("The location must be active to be returned {LocationId}", request.LocationId);

                    return _notificationsHandler
                        .AddNotification("The location must be active to be returned", ENotificationType.BusinessValidation)
                        .ReturnDefault<ReturnLeaseResponse>();
                }

                if (request.DateTime.Date < location.InitialDate.Date)
                {
                    _logger.LogWarning("The location has not yet started {LocationId}", request.LocationId);

                    return _notificationsHandler
                        .AddNotification("The location has not yet started", ENotificationType.BusinessValidation)
                        .ReturnDefault<ReturnLeaseResponse>();
                }

                location.GiveBack(request.DateTime, location.Plan);

                _motorcycleRepository.UpdateLocation(location);
                await _unitOfWork.CommitAsync();

                var @event = _mapper.Map<ReturnedLeaseEvent>(location);

                await _MotorcycleProducer.SendReturnedLeaseEventAsync(@event);

                var response = _mapper.Map<ReturnLeaseResponse>(location);

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred when trying to return the Location: {Exception}", ex.Message);

                return _notificationsHandler
                     .AddNotification("An error occurred when trying to return the Location", ENotificationType.InternalError)
                     .ReturnDefault<ReturnLeaseResponse>();
            }
        }
    }
}
