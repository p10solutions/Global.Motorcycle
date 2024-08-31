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

namespace Global.Motorcycle.Application.Features.Motorcycles.Commands.CreateMotorcycle
{
    public class CreateMotorcycleHandler : IRequestHandler<CreateMotorcycleCommand, CreateMotorcycleResponse>
    {
        readonly IMotorcycleRepository _motorcycleRepository;
        readonly ILogger<CreateMotorcycleHandler> _logger;
        readonly IMapper _mapper;
        readonly INotificationsHandler _notificationsHandler;
        readonly IUnitOfWork _unitOfWork;
        readonly IMotorcycleProducer _motorcycleProducer;

        public CreateMotorcycleHandler(IMotorcycleRepository motorcycleRepository, ILogger<CreateMotorcycleHandler> logger,
            IMapper mapper, INotificationsHandler notificationsHandler, IUnitOfWork unitOfWork, IMotorcycleProducer motorcycleProducer)
        {
            _motorcycleRepository = motorcycleRepository;
            _logger = logger;
            _mapper = mapper;
            _notificationsHandler = notificationsHandler;
            _unitOfWork = unitOfWork;
            _motorcycleProducer = motorcycleProducer;
        }

        public async Task<CreateMotorcycleResponse> Handle(CreateMotorcycleCommand request, CancellationToken cancellationToken)
        {
            var motorcycle = _mapper.Map<MotorcycleEntity>(request);

            try
            {
                if (await _motorcycleRepository.PlateExistsAsync(motorcycle.Plate, motorcycle.Id))
                {
                    _logger.LogWarning("There is already a Motorcycle with that Plate: {MotorcyclePlate}", request.Plate);

                    return _notificationsHandler
                        .AddNotification("There is already a Motorcycle with that Plate", ENotificationType.BusinessValidation)
                        .ReturnDefault<CreateMotorcycleResponse>();
                }

                if (await _motorcycleRepository.ModelExistsAsync(motorcycle.Model, motorcycle.Id))
                {
                    _logger.LogWarning("There is already a Motorcycle with that Model: {MotorcycleModel}", request.Model);

                    return _notificationsHandler
                        .AddNotification("There is already a Motorcycle with that Model", ENotificationType.BusinessValidation)
                        .ReturnDefault<CreateMotorcycleResponse>();
                }

                await _motorcycleRepository.AddAsync(motorcycle);
                await _unitOfWork.CommitAsync();

                var @event = _mapper.Map<CreatedMotorcycleEvent>(motorcycle);

                await _motorcycleProducer.SendCreatedEventAsync(@event);

                var response = _mapper.Map<CreateMotorcycleResponse>(motorcycle);

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred when trying to insert the Motorcycle: {Exception}", ex.Message);

               return _notificationsHandler
                    .AddNotification("An error occurred when trying to insert the Motorcycle", ENotificationType.InternalError)
                    .ReturnDefault<CreateMotorcycleResponse>();
            }
        }
    }
}
