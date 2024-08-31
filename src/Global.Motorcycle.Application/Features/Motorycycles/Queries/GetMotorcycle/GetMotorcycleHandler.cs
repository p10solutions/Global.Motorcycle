using AutoMapper;
using Global.Motorcycle.Domain.Contracts.Data.Repositories;
using Global.Motorcycle.Domain.Contracts.Notifications;
using Global.Motorcycle.Domain.Models.Notifications;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Global.Motorcycle.Application.Features.Motorycycles.Queries.GetMotorcycle
{
    public class GetMotorcycleHandler : IRequestHandler<GetMotorcycleQuery, IEnumerable<GetMotorcycleResponse>>
    {
        readonly IMotorcycleRepository _motorcycleRepository;
        readonly ILogger<GetMotorcycleHandler> _logger;
        readonly INotificationsHandler _notificationsHandler;
        readonly IMapper _mapper;

        public GetMotorcycleHandler(IMotorcycleRepository motorcycleRepository, ILogger<GetMotorcycleHandler> logger,
            INotificationsHandler notificationsHandler, IMapper mapper)
        {
            _motorcycleRepository = motorcycleRepository;
            _logger = logger;
            _notificationsHandler = notificationsHandler;
            _mapper = mapper;
        }

        public async Task<IEnumerable<GetMotorcycleResponse>> Handle(GetMotorcycleQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var motorcycle = await _motorcycleRepository.GetAsync(request.Plate);

                var response = _mapper.Map<IEnumerable<GetMotorcycleResponse>>(motorcycle);

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred when trying to get the Motorcycle: {exception}", ex.Message);

                return _notificationsHandler
                        .AddNotification("An error occurred when trying to get the Motorcycle", ENotificationType.InternalError)
                        .ReturnDefault<IEnumerable<GetMotorcycleResponse>>();
            }
        }
    }
}
