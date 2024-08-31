using AutoFixture;
using AutoMapper;
using Global.Motorcycle.Application.Features.Motorycycles.Queries.GetMotorcycleById;
using Global.Motorcycle.Domain.Contracts.Cache;
using Global.Motorcycle.Domain.Contracts.Data.Repositories;
using Global.Motorcycle.Domain.Contracts.Notifications;
using Global.Motorcycle.Domain.Entities;
using Global.Motorcycle.Domain.Models.Notifications;
using Microsoft.Extensions.Logging;
using Moq;

namespace Global.Motorcycle.UnitTest.Application.Features.Motorcycles.Queries.GetById
{
    public class GetMotorcycleByIdUnitTest
    {
        readonly Mock<IMotorcycleRepository> _MotorcycleRepository;
        readonly Mock<ILogger<GetMotorcycleByIdHandler>> _logger;
        readonly Mock<INotificationsHandler> _notificationsHandler;
        readonly Fixture _fixture;
        readonly GetMotorcycleByIdHandler _handler;
        readonly Mock<IMotorcycleCache> _MotorcycleCache;

        public GetMotorcycleByIdUnitTest()
        {
            _MotorcycleRepository = new Mock<IMotorcycleRepository>();
            _logger = new Mock<ILogger<GetMotorcycleByIdHandler>>();
            _notificationsHandler = new Mock<INotificationsHandler>();
            _fixture = new Fixture();
            _MotorcycleCache = new Mock<IMotorcycleCache>();

            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new GetMotorcycleByIdMapper());
            });
            IMapper mapper = mappingConfig.CreateMapper();

            _handler = new GetMotorcycleByIdHandler(_MotorcycleRepository.Object, _logger.Object, 
                _notificationsHandler.Object, mapper, _MotorcycleCache.Object);
        }

        [Fact]
        public async Task Motorcycle_Should_Be_Geted_Successfully_When_All_Information_Has_Been_Submitted()
        {
            var MotorcycleQuery = _fixture.Create<GetMotorcycleByIdQuery>();
            var Motorcycle = _fixture.Create<MotorcycleEntity>();

            _MotorcycleCache.Setup(x => x.GetAsync(It.IsAny<Guid>())).ReturnsAsync(Motorcycle);

            var response = await _handler.Handle(MotorcycleQuery, CancellationToken.None);

            Assert.NotNull(response);
            _MotorcycleCache.Verify(x => x.GetAsync(It.IsAny<Guid>()), Times.Once);
        }

        [Fact]
        public async Task Motorcycle_Should_Be_Geted_From_Repository()
        {
            var MotorcycleQuery = _fixture.Create<GetMotorcycleByIdQuery>();
            var Motorcycle = _fixture.Create<MotorcycleEntity>();
            _MotorcycleRepository.Setup(x => x.GetAsync(It.IsAny<Guid>())).ReturnsAsync(Motorcycle);

            var response = await _handler.Handle(MotorcycleQuery, CancellationToken.None);

            Assert.NotNull(response);
            _MotorcycleRepository.Verify(x => x.GetAsync(It.IsAny<Guid>()), Times.Once);
            _MotorcycleCache.Verify(x => x.GetAsync(It.IsAny<Guid>()), Times.Once);
            _MotorcycleCache.Verify(x => x.AddAsync(It.IsAny<MotorcycleEntity>()), Times.Once);
        }

        [Fact]
        public async Task Motorcycle_Should_Not_Be_Geted_When_An_Exception_Was_Thrown()
        {
            var MotorcycleQuery = _fixture.Create<GetMotorcycleByIdQuery>();
            _MotorcycleRepository.Setup(x => x.GetAsync(It.IsAny<Guid>())).Throws(new Exception());
            _notificationsHandler
                .Setup(x => x.AddNotification(It.IsAny<string>(), It.IsAny<ENotificationType>(), It.IsAny<object>()))
                    .Returns(_notificationsHandler.Object);

            var response = await _handler.Handle(MotorcycleQuery, CancellationToken.None);

            Assert.Null(response);
            _MotorcycleRepository.Verify(x => x.GetAsync(It.IsAny<Guid>()), Times.Once);
            _MotorcycleCache.Verify(x => x.GetAsync(It.IsAny<Guid>()), Times.Once);
        }
    }
}
