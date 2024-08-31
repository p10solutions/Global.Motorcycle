using AutoFixture;
using AutoMapper;
using Global.Motorcycle.Application.Features.Motorycycles.Queries.GetMotorcycle;
using Global.Motorcycle.Application.Features.Motorycycles.Queries.GetMotorcycleById;
using Global.Motorcycle.Domain.Contracts.Data.Repositories;
using Global.Motorcycle.Domain.Contracts.Notifications;
using Global.Motorcycle.Domain.Entities;
using Global.Motorcycle.Domain.Models.Notifications;
using Microsoft.Extensions.Logging;
using Moq;

namespace Global.Motorcycle.UnitTest.Application.Features.Motorcycles.Queries.Get
{
    public class GetMotorcycleHandlerUnitTest
    {
        readonly Mock<IMotorcycleRepository> _MotorcycleRepository;
        readonly Mock<ILogger<GetMotorcycleHandler>> _logger;
        readonly Mock<INotificationsHandler> _notificationsHandler;
        readonly Fixture _fixture;
        readonly GetMotorcycleHandler _handler;

        public GetMotorcycleHandlerUnitTest()
        {
            _MotorcycleRepository = new Mock<IMotorcycleRepository>();
            _logger = new Mock<ILogger<GetMotorcycleHandler>>();
            _notificationsHandler = new Mock<INotificationsHandler>();
            _fixture = new Fixture();

            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new GetMotorcycleMapper());
            });
            IMapper mapper = mappingConfig.CreateMapper();

            _handler = new GetMotorcycleHandler(_MotorcycleRepository.Object, _logger.Object, _notificationsHandler.Object, mapper);
        }

        [Fact]
        public async Task Motorcycle_Should_Be_Geted_Successfully()
        {
            var MotorcycleQuery = _fixture.Create<GetMotorcycleQuery>();
            var Motorcycle = _fixture.Create<List<MotorcycleEntity>>();

            _MotorcycleRepository.Setup(x => x.GetAsync(It.IsAny<string>())).ReturnsAsync(Motorcycle);

            var response = await _handler.Handle(MotorcycleQuery, CancellationToken.None);

            Assert.NotNull(response);

            _MotorcycleRepository.Verify(x => x.GetAsync(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task Motorcycle_Should_Not_Be_Geted_When_An_Exception_Was_Thrown()
        {
            var MotorcycleQuery = _fixture.Create<GetMotorcycleQuery>();
            _MotorcycleRepository.Setup(x => x.GetAsync(It.IsAny<string>())).Throws(new Exception());
            _notificationsHandler
                .Setup(x => x.AddNotification(It.IsAny<string>(), It.IsAny<ENotificationType>(), It.IsAny<object>()))
                    .Returns(_notificationsHandler.Object);
            _notificationsHandler.Setup(x => x.ReturnDefault<Guid>()).Returns(Guid.Empty);

            var response = await _handler.Handle(MotorcycleQuery, CancellationToken.None);

            Assert.Empty(response);
            _MotorcycleRepository.Verify(x => x.GetAsync(It.IsAny<string>()), Times.Once);
        }
    }
}
