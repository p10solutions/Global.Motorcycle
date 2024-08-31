using Global.Motorcycle.Domain.Contracts.Data;
using Global.Motorcycle.Domain.Contracts.Data.Repositories;
using Global.Motorcycle.Domain.Contracts.Events;
using Global.Motorcycle.Domain.Contracts.Notifications;
using Global.Motorcycle.Domain.Models.Events.Motorcycles;
using Global.Motorcycle.Domain.Models.Notifications;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Global.Motorcycle.Application.Features.Motorycycles.Commands.DeleteMotorcycle
{
    public class DeleteMotorcycleHandler : IRequestHandler<DeleteMotorcycleCommand, DeleteMotorcycleResponse>
    {
        readonly IMotorcycleRepository _motorcycleRepository;
        readonly ILogger<DeleteMotorcycleHandler> _logger;
        readonly INotificationsHandler _notificationsHandler;
        readonly IUnitOfWork _unitOfWork;
        readonly IMotorcycleProducer _motorcycleProducer;

        public DeleteMotorcycleHandler(IMotorcycleRepository motorcycleRepository, ILogger<DeleteMotorcycleHandler> logger,
            INotificationsHandler notificationsHandler, IUnitOfWork unitOfWork,
            IMotorcycleProducer MotorcycleProducer)
        {
            _motorcycleRepository = motorcycleRepository;
            _logger = logger;
            _notificationsHandler = notificationsHandler;
            _unitOfWork = unitOfWork;
            _motorcycleProducer = MotorcycleProducer;
        }

        public async Task<DeleteMotorcycleResponse> Handle(DeleteMotorcycleCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (!await _motorcycleRepository.MotorcycleExistsAsync(request.Id))
                {
                    _logger.LogWarning("Motorcycle not found {MotorcycleId}", request.Id);

                    return _notificationsHandler
                        .AddNotification("Motorcycle not found", ENotificationType.NotFound)
                        .ReturnDefault<DeleteMotorcycleResponse>();
                }

                if (await _motorcycleRepository.LocationActiveExistsAsync(request.Id))
                {
                    _logger.LogWarning("There is an active lease for the motorcycle: {MotorcycleId}", request.Id);

                    return _notificationsHandler
                        .AddNotification("There is an active lease for the motorcycle", ENotificationType.BusinessValidation)
                        .ReturnDefault<DeleteMotorcycleResponse>();
                }

                _motorcycleRepository.Delete(request.Id);
                await _unitOfWork.CommitAsync();

                await _motorcycleProducer.SendDeletedEventAsync(new DeletedMotorcycleEvent(request.Id));

                return new DeleteMotorcycleResponse(request.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred when trying to delete the Motorcycle: {Exception}", ex.Message);

                return _notificationsHandler
                     .AddNotification("An error occurred when trying to delete the Motorcycle", ENotificationType.InternalError)
                     .ReturnDefault<DeleteMotorcycleResponse>();
            }
        }
    }
}
