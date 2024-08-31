using AutoMapper;
using Global.Motorcycle.Domain.Contracts.Data;
using Global.Motorcycle.Domain.Contracts.Data.Repositories;
using Global.Motorcycle.Domain.Contracts.Events;
using Global.Motorcycle.Domain.Contracts.Notifications;
using Global.Motorcycle.Domain.Entities;
using Global.Motorcycle.Domain.Models.Events.Motorcycles;
using Global.Motorcycle.Domain.Models.Notifications;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Global.Motorcycle.Application.Features.Motorcycles.Commands.UpdateMotorcycle
{
    public class UpdateMotorcycleHandler : IRequestHandler<UpdateMotorcycleCommand, UpdateMotorcycleResponse>
    {
        readonly IMotorcycleRepository _motorcycleRepository;
        readonly ILogger<UpdateMotorcycleHandler> _logger;
        readonly IMapper _mapper;
        readonly INotificationsHandler _notificationsHandler;
        readonly IUnitOfWork _unitOfWork;
        readonly IMotorcycleProducer _motorcycleProducer;

        public UpdateMotorcycleHandler(IMotorcycleRepository motorcycleRepository, ILogger<UpdateMotorcycleHandler> logger,
            IMapper mapper, INotificationsHandler notificationsHandler, IUnitOfWork unitOfWork, IMotorcycleProducer motorcycleProducer)
        {
            _motorcycleRepository = motorcycleRepository;
            _logger = logger;
            _mapper = mapper;
            _notificationsHandler = notificationsHandler;
            _unitOfWork = unitOfWork;
            _motorcycleProducer = motorcycleProducer;
        }

        public async Task<UpdateMotorcycleResponse> Handle(UpdateMotorcycleCommand request, CancellationToken cancellationToken)
        {
            var motorcycle = _mapper.Map<MotorcycleEntity>(request);

            try
            {
                if(!await _motorcycleRepository.MotorcycleExistsAsync(request.Id))
                {
                    _logger.LogWarning("Motorcycle not found {MotorcycleId}", request.Id);

                    return _notificationsHandler
                        .AddNotification("Motorcycle not found", ENotificationType.NotFound)
                        .ReturnDefault<UpdateMotorcycleResponse>();
                }

                if (await _motorcycleRepository.PlateExistsAsync(motorcycle.Plate, motorcycle.Id))
                {
                    _logger.LogWarning("There is already a Motorcycle with that Plate: {MotorcyclePlate}", request.Plate);

                    return _notificationsHandler
                        .AddNotification("There is already a Motorcycle with that Plate", ENotificationType.BusinessValidation)
                        .ReturnDefault<UpdateMotorcycleResponse>();
                }

                if (await _motorcycleRepository.ModelExistsAsync(motorcycle.Model, motorcycle.Id))
                {
                    _logger.LogWarning("There is already a Motorcycle with that Model: {MotorcycleModel}", request.Model);

                    return _notificationsHandler
                        .AddNotification("There is already a Motorcycle with that Model", ENotificationType.BusinessValidation)
                        .ReturnDefault<UpdateMotorcycleResponse>();
                }

                _motorcycleRepository.Update(motorcycle);
                await _unitOfWork.CommitAsync();

                var @event = _mapper.Map<UpdatedMotorcycleEvent>(motorcycle);

                await _motorcycleProducer.SendUpdatedEventAsync(@event);

                var response = _mapper.Map<UpdateMotorcycleResponse>(motorcycle);

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred when trying to update the Motorcycle: {Exception}", ex.Message);

                return _notificationsHandler
                     .AddNotification("An error occurred when trying to update the Motorcycle", ENotificationType.InternalError)
                     .ReturnDefault<UpdateMotorcycleResponse>();
            }
        }
    }
}
