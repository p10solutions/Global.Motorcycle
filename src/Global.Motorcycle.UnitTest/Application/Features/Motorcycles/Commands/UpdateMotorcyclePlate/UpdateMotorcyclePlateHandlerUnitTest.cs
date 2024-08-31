using Microsoft.Extensions.Logging;
using Moq;
using AutoFixture;
using Global.Motorcycle.Domain.Models.Notifications;
using Global.Motorcycle.Domain.Contracts.Notifications;
using Global.Motorcycle.Domain.Contracts.Events;
using Global.Motorcycle.Domain.Contracts.Data.Repositories;
using Global.Motorcycle.Domain.Contracts.Data;
using AutoMapper;
using Global.Motorcycle.Domain.Models.Events.Motorcycles;
using Global.Motorcycle.Application.Features.Motorycycles.Commands.UpdateMotorcyclePlate;
using Global.Motorcycle.Domain.Entities;
using Global.Motorcycle.Application.Features.Motorcycles.Commands.UpdateMotorcycle;

namespace Global.Motorcycle.UnitTest.Application.Features.Motorcycles.Commands.UpdateMotorcyclePlate
{
    public class UpdateMotorcyclePlateHandlerUnitTest
    {
        readonly Mock<IMotorcycleRepository> _MotorcycleRepository;
        readonly Mock<IMotorcycleProducer> _MotorcycleProducer;
        readonly Mock<ILogger<UpdateMotorcyclePlateHandler>> _logger;
        readonly Mock<INotificationsHandler> _notificationsHandler;
        readonly Mock<IUnitOfWork> _unitOfWork;
        readonly Fixture _fixture;
        readonly UpdateMotorcyclePlateHandler _handler;

        public UpdateMotorcyclePlateHandlerUnitTest()
        {
            _MotorcycleRepository = new Mock<IMotorcycleRepository>();
            _MotorcycleProducer = new Mock<IMotorcycleProducer>();
            _logger = new Mock<ILogger<UpdateMotorcyclePlateHandler>>();
            _notificationsHandler = new Mock<INotificationsHandler>();
            _unitOfWork = new Mock<IUnitOfWork>();
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new UpdateMotorcyclePlateMapper());
                mc.AddProfile(new UpdateMotorcycleMapper());
            });
            IMapper mapper = mappingConfig.CreateMapper();

            _fixture = new Fixture();
            _handler = new UpdateMotorcyclePlateHandler(_MotorcycleRepository.Object, _logger.Object, mapper,
                _notificationsHandler.Object, _unitOfWork.Object, _MotorcycleProducer.Object);
        }

        [Fact]
        public async Task Motorcycle_Should_Be_Updated_Successfully_When_All_Information_Has_Been_Submitted()
        {
            var MotorcycleCommand = _fixture.Create<UpdateMotorcyclePlateCommand>();
            var motorcycle = _fixture.Create<MotorcycleEntity>();

            _MotorcycleRepository.Setup(x => x.PlateExistsAsync(It.IsAny<string>(), It.IsAny<Guid>())).ReturnsAsync(false);
            _MotorcycleRepository.Setup(x => x.GetAsync(It.IsAny<Guid>())).ReturnsAsync(motorcycle);
            _MotorcycleRepository.Setup(x => x.MotorcycleExistsAsync(It.IsAny<Guid>())).ReturnsAsync(true);

            var response = await _handler.Handle(MotorcycleCommand, CancellationToken.None);

            Assert.NotNull(response);
            _MotorcycleRepository.Verify(x => x.UpdatePlate(It.IsAny<Guid>(), It.IsAny<string>()), Times.Once);
            _MotorcycleProducer.Verify(x => x.SendUpdatedEventAsync(It.IsAny<UpdatedMotorcycleEvent>()), Times.Once);
            _unitOfWork.Verify(x => x.CommitAsync(), Times.Once);
            _MotorcycleRepository.Verify(x => x.PlateExistsAsync(It.IsAny<string>(), It.IsAny<Guid>()), Times.Once);
            _MotorcycleRepository.Verify(x => x.MotorcycleExistsAsync(It.IsAny<Guid>()), Times.Once);
        }

        [Fact]
        public async Task Motorcycle_Should_Not_Be_Updated_When_An_Exception_Was_Thrown()
        {
            var MotorcycleCommand = _fixture.Create<UpdateMotorcyclePlateCommand>();
            _MotorcycleRepository.Setup(x => x.UpdatePlate(It.IsAny<Guid>(), It.IsAny<string>())).Throws(new Exception());
            _MotorcycleRepository.Setup(x => x.PlateExistsAsync(It.IsAny<string>(), It.IsAny<Guid>())).ReturnsAsync(false);
            _MotorcycleRepository.Setup(x => x.MotorcycleExistsAsync(It.IsAny<Guid>())).ReturnsAsync(true);
            _notificationsHandler
                .Setup(x => x.AddNotification(It.IsAny<string>(), It.IsAny<ENotificationType>(), It.IsAny<object>()))
                    .Returns(_notificationsHandler.Object);

            var response = await _handler.Handle(MotorcycleCommand, CancellationToken.None);

            Assert.Null(response);

            _MotorcycleRepository.Verify(x => x.UpdatePlate(It.IsAny<Guid>(), It.IsAny<string>()), Times.Once);
            _MotorcycleProducer.Verify(x => x.SendUpdatedEventAsync(It.IsAny<UpdatedMotorcycleEvent>()), Times.Never);
            _unitOfWork.Verify(x => x.CommitAsync(), Times.Never);
            _MotorcycleRepository.Verify(x => x.PlateExistsAsync(It.IsAny<string>(), It.IsAny<Guid>()), Times.Once);
            _MotorcycleRepository.Verify(x => x.MotorcycleExistsAsync(It.IsAny<Guid>()), Times.Once);
        }


        [Fact]
        public async Task Motorcycle_Should_Not_Be_Updated_When_Plate_Already_Exists()
        {
            var MotorcycleCommand = _fixture.Create<UpdateMotorcyclePlateCommand>();
            _MotorcycleRepository.Setup(x => x.PlateExistsAsync(It.IsAny<string>(), It.IsAny<Guid>())).ReturnsAsync(true);
            _MotorcycleRepository.Setup(x => x.MotorcycleExistsAsync(It.IsAny<Guid>())).ReturnsAsync(true);
            _notificationsHandler
                .Setup(x => x.AddNotification(It.IsAny<string>(), It.IsAny<ENotificationType>(), It.IsAny<object>()))
                    .Returns(_notificationsHandler.Object);

            var response = await _handler.Handle(MotorcycleCommand, CancellationToken.None);

            Assert.Null(response);

            _MotorcycleRepository.Verify(x => x.UpdatePlate(It.IsAny<Guid>(), It.IsAny<string>()), Times.Never);
            _MotorcycleProducer.Verify(x => x.SendUpdatedEventAsync(It.IsAny<UpdatedMotorcycleEvent>()), Times.Never);
            _unitOfWork.Verify(x => x.CommitAsync(), Times.Never);
            _MotorcycleRepository.Verify(x => x.PlateExistsAsync(It.IsAny<string>(), It.IsAny<Guid>()), Times.Once);
            _MotorcycleRepository.Verify(x => x.MotorcycleExistsAsync(It.IsAny<Guid>()), Times.Once);
        }


        [Fact]
        public async Task Motorcycle_Should_Not_Be_Updated_When_Not_Found()
        {
            var MotorcycleCommand = _fixture.Create<UpdateMotorcyclePlateCommand>();
            _MotorcycleRepository.Setup(x => x.PlateExistsAsync(It.IsAny<string>(), It.IsAny<Guid>())).ReturnsAsync(false);
            _MotorcycleRepository.Setup(x => x.MotorcycleExistsAsync(It.IsAny<Guid>())).ReturnsAsync(false);
            _notificationsHandler
                .Setup(x => x.AddNotification(It.IsAny<string>(), It.IsAny<ENotificationType>(), It.IsAny<object>()))
                    .Returns(_notificationsHandler.Object);

            var response = await _handler.Handle(MotorcycleCommand, CancellationToken.None);

            Assert.Null(response);

            _MotorcycleRepository.Verify(x => x.UpdatePlate(It.IsAny<Guid>(), It.IsAny<string>()), Times.Never);
            _MotorcycleProducer.Verify(x => x.SendUpdatedEventAsync(It.IsAny<UpdatedMotorcycleEvent>()), Times.Never);
            _unitOfWork.Verify(x => x.CommitAsync(), Times.Never);
            _MotorcycleRepository.Verify(x => x.PlateExistsAsync(It.IsAny<string>(), It.IsAny<Guid>()), Times.Never);
            _MotorcycleRepository.Verify(x => x.MotorcycleExistsAsync(It.IsAny<Guid>()), Times.Once);
        }
    }
}
