using AutoMapper;
using Global.Motorcycle.Domain.Contracts.Data;
using Global.Motorcycle.Domain.Contracts.Data.Repositories;
using Global.Motorcycle.Domain.Contracts.Events;
using Global.Motorcycle.Domain.Contracts.Notifications;
using Global.Motorcycle.Domain.Models.Events.Motorcycles;
using Global.Motorcycle.Domain.Models.Notifications;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Global.Motorcycle.Application.Features.Motorycycles.Commands.UpdateMotorcyclePlate
{
    public class UpdateMotorcyclePlateHandler : IRequestHandler<UpdateMotorcyclePlateCommand, UpdateMotorcyclePlateResponse>
    {
        readonly IMotorcycleRepository _motorcycleRepository;
        readonly ILogger<UpdateMotorcyclePlateHandler> _logger;
        readonly IMapper _mapper;
        readonly INotificationsHandler _notificationsHandler;
        readonly IUnitOfWork _unitOfWork;
        readonly IMotorcycleProducer _motorcycleProducer;

        public UpdateMotorcyclePlateHandler(IMotorcycleRepository motorcycleRepository, ILogger<UpdateMotorcyclePlateHandler> logger,
            IMapper mapper, INotificationsHandler notificationsHandler, IUnitOfWork unitOfWork,
            IMotorcycleProducer motorcycleProducer)
        {
            _motorcycleRepository = motorcycleRepository;
            _logger = logger;
            _mapper = mapper;
            _notificationsHandler = notificationsHandler;
            _unitOfWork = unitOfWork;
            _motorcycleProducer = motorcycleProducer;
        }

        public async Task<UpdateMotorcyclePlateResponse> Handle(UpdateMotorcyclePlateCommand request, CancellationToken cancellationToken)
        {
            try
            {

                if (!await _motorcycleRepository.MotorcycleExistsAsync(request.Id))
                {
                    _logger.LogWarning("Motorcycle not found {MotorcycleId}", request.Id);

                    return _notificationsHandler
                        .AddNotification("Motorcycle not found", ENotificationType.NotFound)
                        .ReturnDefault<UpdateMotorcyclePlateResponse>();
                }

                if (await _motorcycleRepository.PlateExistsAsync(request.Plate, request.Id))
                {
                    _logger.LogWarning("There is already a Motorcycle with that Plate: {MotorcyclePlate}", request.Plate);

                    return _notificationsHandler
                        .AddNotification("There is already a Motorcycle with that Plate", ENotificationType.BusinessValidation)
                        .ReturnDefault<UpdateMotorcyclePlateResponse>();
                }

                var motorcycle = await _motorcycleRepository.GetAsync(request.Id);

                motorcycle.Plate = request.Plate;

                _motorcycleRepository.Update(motorcycle);
                await _unitOfWork.CommitAsync();

                var @event = _mapper.Map<UpdatedMotorcycleEvent>(motorcycle);

                await _motorcycleProducer.SendUpdatedEventAsync(@event);

                var response = _mapper.Map<UpdateMotorcyclePlateResponse>(motorcycle);

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred when trying to update the Motorcycle: {Exception}", ex.Message);

                return _notificationsHandler
                     .AddNotification("An error occurred when trying to update the Motorcycle", ENotificationType.InternalError)
                     .ReturnDefault<UpdateMotorcyclePlateResponse>();
            }
        }
    }
}
