using Microsoft.Extensions.Logging;
using Moq;
using AutoFixture;
using Global.Motorcycle.Domain.Models.Notifications;
using Global.Motorcycle.Domain.Contracts.Notifications;
using Global.Motorcycle.Domain.Contracts.Events;
using Global.Motorcycle.Application.Features.Motorcycles.Commands.CreateMotorcycle;
using Global.Motorcycle.Domain.Contracts.Data.Repositories;
using Global.Motorcycle.Domain.Contracts.Data;
using AutoMapper;
using Global.Motorcycle.Domain.Models.Events.Motorcycles;
using Global.Motorcycle.Domain.Entities;

namespace Global.Motorcycle.UnitTest.Application.Features.Motorcycles.Commands.CreateMotorcycle
{
    public class CreateMotorcycleHandlerUnitTest
    {
        readonly Mock<IMotorcycleRepository> _MotorcycleRepository;
        readonly Mock<IMotorcycleProducer> _MotorcycleProducer;
        readonly Mock<ILogger<CreateMotorcycleHandler>> _logger;
        readonly Mock<INotificationsHandler> _notificationsHandler;
        readonly Mock<IUnitOfWork> _unitOfWork;
        readonly Fixture _fixture;
        readonly CreateMotorcycleHandler _handler;

        public CreateMotorcycleHandlerUnitTest()
        {
            _MotorcycleRepository = new Mock<IMotorcycleRepository>();
            _MotorcycleProducer = new Mock<IMotorcycleProducer>();
            _logger = new Mock<ILogger<CreateMotorcycleHandler>>();
            _notificationsHandler = new Mock<INotificationsHandler>();
            _unitOfWork = new Mock<IUnitOfWork>();
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new CreateMotorcycleMapper());
            });
            IMapper mapper = mappingConfig.CreateMapper();

            _fixture = new Fixture();
            _handler = new CreateMotorcycleHandler(_MotorcycleRepository.Object, _logger.Object, mapper,
                _notificationsHandler.Object, _unitOfWork.Object, _MotorcycleProducer.Object);
        }

        [Fact]
        public async Task Motorcycle_Should_Be_Created_Successfully_When_All_Information_Has_Been_Submitted()
        {
            var MotorcycleCommand = _fixture.Create<CreateMotorcycleCommand>();

            _MotorcycleRepository.Setup(x => x.ModelExistsAsync(It.IsAny<string>(), It.IsAny<Guid>())).ReturnsAsync(false);
            _MotorcycleRepository.Setup(x => x.PlateExistsAsync(It.IsAny<string>(), It.IsAny<Guid>())).ReturnsAsync(false);

            var response = await _handler.Handle(MotorcycleCommand, CancellationToken.None);

            Assert.NotNull(response);
            _MotorcycleRepository.Verify(x => x.AddAsync(It.IsAny<MotorcycleEntity>()), Times.Once);
            _MotorcycleProducer.Verify(x => x.SendCreatedEventAsync(It.IsAny<CreatedMotorcycleEvent>()), Times.Once);
            _unitOfWork.Verify(x => x.CommitAsync(), Times.Once);
            _MotorcycleRepository.Verify(x => x.ModelExistsAsync(It.IsAny<string>(), It.IsAny<Guid>()), Times.Once);
            _MotorcycleRepository.Verify(x => x.PlateExistsAsync(It.IsAny<string>(), It.IsAny<Guid>()), Times.Once);
        }

        [Fact]
        public async Task Motorcycle_Should_Not_Be_Created_When_An_Exception_Was_Thrown()
        {
            var MotorcycleCommand = _fixture.Create<CreateMotorcycleCommand>();

            _MotorcycleRepository.Setup(x => x.ModelExistsAsync(It.IsAny<string>(), It.IsAny<Guid>())).ReturnsAsync(false);
            _MotorcycleRepository.Setup(x => x.PlateExistsAsync(It.IsAny<string>(), It.IsAny<Guid>())).ReturnsAsync(false);
            _MotorcycleRepository.Setup(x => x.AddAsync(It.IsAny<MotorcycleEntity>())).Throws(new Exception());
            _notificationsHandler
                .Setup(x => x.AddNotification(It.IsAny<string>(), It.IsAny<ENotificationType>(), It.IsAny<object>()))
                    .Returns(_notificationsHandler.Object);

            var response = await _handler.Handle(MotorcycleCommand, CancellationToken.None);

            Assert.Null(response);

            _MotorcycleRepository.Verify(x => x.AddAsync(It.IsAny<MotorcycleEntity>()), Times.Once);
            _MotorcycleProducer.Verify(x => x.SendCreatedEventAsync(It.IsAny<CreatedMotorcycleEvent>()), Times.Never);
            _unitOfWork.Verify(x => x.CommitAsync(), Times.Never);
            _MotorcycleRepository.Verify(x => x.ModelExistsAsync(It.IsAny<string>(), It.IsAny<Guid>()), Times.Once);
            _MotorcycleRepository.Verify(x => x.PlateExistsAsync(It.IsAny<string>(), It.IsAny<Guid>()), Times.Once);
        }

        [Fact]
        public async Task Motorcycle_Should_Not_Be_Created_When_Model_Already_Exists()
        {
            var MotorcycleCommand = _fixture.Create<CreateMotorcycleCommand>();

            _MotorcycleRepository.Setup(x => x.ModelExistsAsync(It.IsAny<string>(), It.IsAny<Guid>())).ReturnsAsync(true);
            _MotorcycleRepository.Setup(x => x.PlateExistsAsync(It.IsAny<string>(), It.IsAny<Guid>())).ReturnsAsync(false);
            _MotorcycleRepository.Setup(x => x.AddAsync(It.IsAny<MotorcycleEntity>())).Throws(new Exception());
            _notificationsHandler
                .Setup(x => x.AddNotification(It.IsAny<string>(), It.IsAny<ENotificationType>(), It.IsAny<object>()))
                    .Returns(_notificationsHandler.Object);

            var response = await _handler.Handle(MotorcycleCommand, CancellationToken.None);

            Assert.Null(response);

            _MotorcycleRepository.Verify(x => x.AddAsync(It.IsAny<MotorcycleEntity>()), Times.Never);
            _MotorcycleProducer.Verify(x => x.SendCreatedEventAsync(It.IsAny<CreatedMotorcycleEvent>()), Times.Never);
            _unitOfWork.Verify(x => x.CommitAsync(), Times.Never);
            _MotorcycleRepository.Verify(x => x.ModelExistsAsync(It.IsAny<string>(), It.IsAny<Guid>()), Times.Once);
            _MotorcycleRepository.Verify(x => x.PlateExistsAsync(It.IsAny<string>(), It.IsAny<Guid>()), Times.Once);
        }

        [Fact]
        public async Task Motorcycle_Should_Not_Be_Created_When_Plate_Already_Exists()
        {
            var MotorcycleCommand = _fixture.Create<CreateMotorcycleCommand>();

            _MotorcycleRepository.Setup(x => x.ModelExistsAsync(It.IsAny<string>(), It.IsAny<Guid>())).ReturnsAsync(false);
            _MotorcycleRepository.Setup(x => x.PlateExistsAsync(It.IsAny<string>(), It.IsAny<Guid>())).ReturnsAsync(true);
            _notificationsHandler
                .Setup(x => x.AddNotification(It.IsAny<string>(), It.IsAny<ENotificationType>(), It.IsAny<object>()))
                    .Returns(_notificationsHandler.Object);

            var response = await _handler.Handle(MotorcycleCommand, CancellationToken.None);

            Assert.Null(response);

            _MotorcycleRepository.Verify(x => x.AddAsync(It.IsAny<MotorcycleEntity>()), Times.Never);
            _MotorcycleProducer.Verify(x => x.SendCreatedEventAsync(It.IsAny<CreatedMotorcycleEvent>()), Times.Never);
            _unitOfWork.Verify(x => x.CommitAsync(), Times.Never);
            _MotorcycleRepository.Verify(x => x.ModelExistsAsync(It.IsAny<string>(), It.IsAny<Guid>()), Times.Never);
            _MotorcycleRepository.Verify(x => x.PlateExistsAsync(It.IsAny<string>(), It.IsAny<Guid>()), Times.Once);
        }
    }
}
