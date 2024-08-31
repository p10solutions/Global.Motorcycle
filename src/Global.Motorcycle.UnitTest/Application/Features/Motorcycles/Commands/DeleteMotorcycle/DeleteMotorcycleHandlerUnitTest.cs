using Microsoft.Extensions.Logging;
using Moq;
using AutoFixture;
using Global.Motorcycle.Domain.Models.Notifications;
using Global.Motorcycle.Domain.Contracts.Notifications;
using Global.Motorcycle.Domain.Contracts.Events;
using Global.Motorcycle.Domain.Contracts.Data.Repositories;
using Global.Motorcycle.Domain.Contracts.Data;
using Global.Motorcycle.Domain.Models.Events.Motorcycles;
using Global.Motorcycle.Application.Features.Motorycycles.Commands.DeleteMotorcycle;

namespace Global.Motorcycle.UnitTest.Application.Features.Motorcycles.Commands.DeleteMotorcycle
{
    public class DeleteMotorcycleHandlerUnitTest
    {
        readonly Mock<IMotorcycleRepository> _MotorcycleRepository;
        readonly Mock<IMotorcycleProducer> _MotorcycleProducer;
        readonly Mock<ILogger<DeleteMotorcycleHandler>> _logger;
        readonly Mock<INotificationsHandler> _notificationsHandler;
        readonly Mock<IUnitOfWork> _unitOfWork;
        readonly Fixture _fixture;
        readonly DeleteMotorcycleHandler _handler;

        public DeleteMotorcycleHandlerUnitTest()
        {
            _MotorcycleRepository = new Mock<IMotorcycleRepository>();
            _MotorcycleProducer = new Mock<IMotorcycleProducer>();
            _logger = new Mock<ILogger<DeleteMotorcycleHandler>>();
            _notificationsHandler = new Mock<INotificationsHandler>();
            _unitOfWork = new Mock<IUnitOfWork>();

            _fixture = new Fixture();
            _handler = new DeleteMotorcycleHandler(_MotorcycleRepository.Object, _logger.Object,
                _notificationsHandler.Object, _unitOfWork.Object, _MotorcycleProducer.Object);
        }

        [Fact]
        public async Task Motorcycle_Should_Be_Deleted_Successfully()
        {
            var MotorcycleCommand = _fixture.Create<DeleteMotorcycleCommand>();

            _MotorcycleRepository.Setup(x => x.LocationActiveExistsAsync(It.IsAny<Guid>())).ReturnsAsync(false);
            _MotorcycleRepository.Setup(x => x.MotorcycleExistsAsync(It.IsAny<Guid>())).ReturnsAsync(true);

            var response = await _handler.Handle(MotorcycleCommand, CancellationToken.None);

            Assert.NotNull(response);
            _MotorcycleRepository.Verify(x => x.Delete(It.IsAny<Guid>()), Times.Once);
            _MotorcycleProducer.Verify(x => x.SendDeletedEventAsync(It.IsAny<DeletedMotorcycleEvent>()), Times.Once);
            _unitOfWork.Verify(x => x.CommitAsync(), Times.Once);
            _MotorcycleRepository.Verify(x => x.LocationActiveExistsAsync(It.IsAny<Guid>()), Times.Once);
            _MotorcycleRepository.Verify(x => x.MotorcycleExistsAsync(It.IsAny<Guid>()), Times.Once);
        }

        [Fact]
        public async Task Motorcycle_Should_Not_Be_Deleted_When_An_Exception_Was_Thrown()
        {
            var MotorcycleCommand = _fixture.Create<DeleteMotorcycleCommand>();

            _MotorcycleRepository.Setup(x => x.LocationActiveExistsAsync(It.IsAny<Guid>())).ReturnsAsync(false);
            _MotorcycleRepository.Setup(x => x.MotorcycleExistsAsync(It.IsAny<Guid>())).ReturnsAsync(true);
            _MotorcycleRepository.Setup(x=>x.Delete(It.IsAny<Guid>())).Throws(new Exception());

            _notificationsHandler
                .Setup(x => x.AddNotification(It.IsAny<string>(), It.IsAny<ENotificationType>(), It.IsAny<object>()))
                 .Returns(_notificationsHandler.Object);

            var response = await _handler.Handle(MotorcycleCommand, CancellationToken.None);

            Assert.Null(response);
            _MotorcycleRepository.Verify(x => x.Delete(It.IsAny<Guid>()), Times.Once);
            _MotorcycleProducer.Verify(x => x.SendDeletedEventAsync(It.IsAny<DeletedMotorcycleEvent>()), Times.Never);
            _unitOfWork.Verify(x => x.CommitAsync(), Times.Never);
            _MotorcycleRepository.Verify(x => x.LocationActiveExistsAsync(It.IsAny<Guid>()), Times.Once);
            _MotorcycleRepository.Verify(x => x.MotorcycleExistsAsync(It.IsAny<Guid>()), Times.Once);
        }


        [Fact]
        public async Task Motorcycle_Should_Not_Be_Deleted_When_A_Location_Exists()
        {
            var MotorcycleCommand = _fixture.Create<DeleteMotorcycleCommand>();

            _MotorcycleRepository.Setup(x => x.LocationActiveExistsAsync(It.IsAny<Guid>())).ReturnsAsync(true);
            _MotorcycleRepository.Setup(x => x.MotorcycleExistsAsync(It.IsAny<Guid>())).ReturnsAsync(true);
            _notificationsHandler
                .Setup(x => x.AddNotification(It.IsAny<string>(), It.IsAny<ENotificationType>(), It.IsAny<object>()))
                .Returns(_notificationsHandler.Object);

            var response = await _handler.Handle(MotorcycleCommand, CancellationToken.None);

            Assert.Null(response);
            _MotorcycleRepository.Verify(x => x.Delete(It.IsAny<Guid>()), Times.Never);
            _MotorcycleProducer.Verify(x => x.SendDeletedEventAsync(It.IsAny<DeletedMotorcycleEvent>()), Times.Never);
            _unitOfWork.Verify(x => x.CommitAsync(), Times.Never);
            _MotorcycleRepository.Verify(x => x.LocationActiveExistsAsync(It.IsAny<Guid>()), Times.Once);
            _MotorcycleRepository.Verify(x => x.MotorcycleExistsAsync(It.IsAny<Guid>()), Times.Once);
        }

        [Fact]
        public async Task Motorcycle_Should_Not_Be_Deleted_When_Not_Found()
        {
            var MotorcycleCommand = _fixture.Create<DeleteMotorcycleCommand>();

            _MotorcycleRepository.Setup(x => x.LocationActiveExistsAsync(It.IsAny<Guid>())).ReturnsAsync(false);
            _MotorcycleRepository.Setup(x => x.MotorcycleExistsAsync(It.IsAny<Guid>())).ReturnsAsync(false);
            _notificationsHandler
                .Setup(x => x.AddNotification(It.IsAny<string>(), It.IsAny<ENotificationType>(), It.IsAny<object>()))
                .Returns(_notificationsHandler.Object);

            var response = await _handler.Handle(MotorcycleCommand, CancellationToken.None);

            Assert.Null(response);
            _MotorcycleRepository.Verify(x => x.Delete(It.IsAny<Guid>()), Times.Never);
            _MotorcycleProducer.Verify(x => x.SendDeletedEventAsync(It.IsAny<DeletedMotorcycleEvent>()), Times.Never);
            _unitOfWork.Verify(x => x.CommitAsync(), Times.Never);
            _MotorcycleRepository.Verify(x => x.LocationActiveExistsAsync(It.IsAny<Guid>()), Times.Never);
            _MotorcycleRepository.Verify(x => x.MotorcycleExistsAsync(It.IsAny<Guid>()), Times.Once);
        }
    }
}
