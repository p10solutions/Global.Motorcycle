using AutoMapper;
using Global.Motorcycle.Domain.Contracts.Cache;
using Global.Motorcycle.Domain.Contracts.Data.Repositories;
using Global.Motorcycle.Domain.Contracts.Notifications;
using Global.Motorcycle.Domain.Models.Notifications;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Global.Motorcycle.Application.Features.Motorycycles.Queries.GetMotorcycleById
{
    public class GetMotorcycleByIdHandler : IRequestHandler<GetMotorcycleByIdQuery, GetMotorcycleByIdResponse>
    {
        readonly IMotorcycleRepository _motorcycleRepository;
        readonly ILogger<GetMotorcycleByIdHandler> _logger;
        readonly INotificationsHandler _notificationsHandler;
        readonly IMapper _mapper;
        readonly IMotorcycleCache _motorcycleCache;

        public GetMotorcycleByIdHandler(IMotorcycleRepository MotorcycleRepository, ILogger<GetMotorcycleByIdHandler> logger,
            INotificationsHandler notificationsHandler, IMapper mapper, IMotorcycleCache motorcycleCache)
        {
            _motorcycleRepository = MotorcycleRepository;
            _logger = logger;
            _notificationsHandler = notificationsHandler;
            _mapper = mapper;
            _motorcycleCache = motorcycleCache;
        }

        public async Task<GetMotorcycleByIdResponse> Handle(GetMotorcycleByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var motorcycle = await _motorcycleCache.GetAsync(request.Id);

                if (motorcycle == null)
                {
                    motorcycle = await _motorcycleRepository.GetAsync(request.Id);

                    if (motorcycle == null)
                    {
                        _logger.LogWarning("Motorcycle: {MotorcycleId} not found", request.Id);

                        return _notificationsHandler
                            .AddNotification("Motorcycle not found", ENotificationType.NotFound)
                            .ReturnDefault<GetMotorcycleByIdResponse>();
                    }

                    await _motorcycleCache.AddAsync(motorcycle);
                }

                var response = _mapper.Map<GetMotorcycleByIdResponse>(motorcycle);

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred when trying to get the Motorcycle: {exception}", ex.Message);

                return _notificationsHandler
                        .AddNotification("An error occurred when trying to get the Motorcycle", ENotificationType.InternalError)
                        .ReturnDefault<GetMotorcycleByIdResponse>();
            }
        }
    }
}
