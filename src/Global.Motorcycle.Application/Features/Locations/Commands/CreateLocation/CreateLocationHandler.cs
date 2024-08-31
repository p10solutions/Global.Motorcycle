using AutoMapper;
using Global.Motorcycle.Domain.Contracts.Data;
using Global.Motorcycle.Domain.Contracts.Data.Repositories;
using Global.Motorcycle.Domain.Contracts.Date;
using Global.Motorcycle.Domain.Contracts.Events;
using Global.Motorcycle.Domain.Contracts.ExternalServices;
using Global.Motorcycle.Domain.Contracts.Notifications;
using Global.Motorcycle.Domain.Entities;
using Global.Motorcycle.Domain.Models.Events.Locations;
using Global.Motorcycle.Domain.Models.ExternalServices.Delivery.Deliveryman;
using Global.Motorcycle.Domain.Models.Notifications;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Global.Motorcycle.Application.Features.Locations.Commands.CreateLocation
{
    public class CreateLocationHandler : IRequestHandler<CreateLocationCommand, CreateLocationResponse>
    {
        readonly IMotorcycleRepository _motorcycleRepository;
        readonly ILogger<CreateLocationHandler> _logger;
        readonly IMapper _mapper;
        readonly INotificationsHandler _notificationsHandler;
        readonly IUnitOfWork _unitOfWork;
        readonly ILocationProducer _locationProducer;
        readonly IDeliveryExternalService _deliveryExternalService;
        readonly ISystemDate _systemDate;

        public CreateLocationHandler(IMotorcycleRepository motorcycleRepository, ILogger<CreateLocationHandler> logger,
            IMapper mapper, INotificationsHandler notificationsHandler, IUnitOfWork unitOfWork,
            ILocationProducer locationProducer, IDeliveryExternalService deliveryExternalService, ISystemDate systemDate)
        {
            _motorcycleRepository = motorcycleRepository;
            _logger = logger;
            _mapper = mapper;
            _notificationsHandler = notificationsHandler;
            _unitOfWork = unitOfWork;
            _locationProducer = locationProducer;
            _deliveryExternalService = deliveryExternalService;
            _systemDate = systemDate;
        }

        public async Task<CreateLocationResponse> Handle(CreateLocationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var licenseTypeResponse = await _deliveryExternalService.GetLicenseTypeAsync(request.DeliverymanId);

                if (!CheckLicense(licenseTypeResponse.LicenseType))
                {
                    _logger.LogWarning("The deliveryman must have a type A license {DeliverymanId}", request.DeliverymanId);

                    return _notificationsHandler
                        .AddNotification("The deliveryman must have a type A license", ENotificationType.BusinessValidation)
                        .ReturnDefault<CreateLocationResponse>();
                }

                var plan = await _motorcycleRepository.GetPlanAsync(request.PlanId);

                if (plan is null)
                {
                    _logger.LogWarning("plan was not found {PlanId}", request.PlanId);

                    return _notificationsHandler
                        .AddNotification("plan was not found", ENotificationType.NotFound)
                        .ReturnDefault<CreateLocationResponse>();
                }

                if (!await _motorcycleRepository.MotorcycleExistsAsync(request.MotorcycleId))
                {
                    _logger.LogWarning("Motorcycle was not found {MotorcycleId}", request.MotorcycleId);

                    return _notificationsHandler
                        .AddNotification("Motorcycle was not found", ENotificationType.NotFound)
                        .ReturnDefault<CreateLocationResponse>();
                }

                if(await _motorcycleRepository.LocationActiveExistsAsync(request.MotorcycleId))
                {
                    _logger.LogWarning("There is an active lease for the motorcycle: {MotorcycleId}", request.MotorcycleId);

                    return _notificationsHandler
                        .AddNotification("There is an active lease for the motorcycle", ENotificationType.BusinessValidation)
                        .ReturnDefault<CreateLocationResponse>();
                }

                var location = _mapper.Map<Location>(request);

                location
                    .SetInitialDate(_systemDate)
                    .SetPlan(plan);

                await _motorcycleRepository.AddLocationAsync(location);
                await _unitOfWork.CommitAsync();

                var @event = _mapper.Map<CreatedLocationEvent>(location);

                await _locationProducer.SendCreatedEventAsync(@event);

                var response = _mapper.Map<CreateLocationResponse>(location);

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred when trying to insert the Location: {Exception}", ex.Message);

                return _notificationsHandler
                     .AddNotification("An error occurred when trying to insert the Location", ENotificationType.InternalError)
                     .ReturnDefault<CreateLocationResponse>();
            }
        }

        private bool CheckLicense(ELicenseType licenseType)
            => licenseType == ELicenseType.A || licenseType == ELicenseType.AB;
    }
}
