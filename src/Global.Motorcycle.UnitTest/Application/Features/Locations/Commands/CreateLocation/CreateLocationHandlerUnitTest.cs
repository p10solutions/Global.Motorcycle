using Microsoft.Extensions.Logging;
using Moq;
using AutoFixture;
using Global.Motorcycle.Domain.Models.Notifications;
using Global.Motorcycle.Domain.Contracts.Notifications;
using Global.Motorcycle.Domain.Contracts.Events;
using Global.Motorcycle.Domain.Contracts.Data.Repositories;
using Global.Motorcycle.Domain.Contracts.Data;
using AutoMapper;
using Global.Motorcycle.Domain.Entities;
using Global.Motorcycle.Application.Features.Locations.Commands.CreateLocation;
using Global.Motorcycle.Domain.Contracts.ExternalServices;
using Global.Motorcycle.Domain.Models.ExternalServices.Delivery.Deliveryman;
using LocationEntity = Global.Motorcycle.Domain.Entities.Location;
using Global.Motorcycle.Domain.Contracts.Date;

namespace Global.Motorcycle.UnitTest.Application.Features.Locations.Commands.CreateLocation
{
    public class CreateLocationHandlerUnitTest
    {
        readonly Mock<IMotorcycleRepository> _MotorcycleRepository;
        readonly Mock<ILocationProducer> _MotorcycleProducer;
        readonly Mock<ILogger<CreateLocationHandler>> _logger;
        readonly Mock<INotificationsHandler> _notificationsHandler;
        readonly Mock<IUnitOfWork> _unitOfWork;
        readonly Fixture _fixture;
        readonly CreateLocationHandler _handler;
        readonly Mock<IDeliveryExternalService> _deliveryExternalService;
        readonly Mock<ISystemDate> _systemDate;

        public CreateLocationHandlerUnitTest()
        {
            _MotorcycleRepository = new Mock<IMotorcycleRepository>();
            _MotorcycleProducer = new Mock<ILocationProducer>();
            _logger = new Mock<ILogger<CreateLocationHandler>>();
            _notificationsHandler = new Mock<INotificationsHandler>();
            _unitOfWork = new Mock<IUnitOfWork>();
            _deliveryExternalService = new Mock<IDeliveryExternalService>();
            _systemDate = new Mock<ISystemDate>();
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new CreateLocationMapper());
            });
            IMapper mapper = mappingConfig.CreateMapper();

            _fixture = new Fixture();
            _handler = new CreateLocationHandler(_MotorcycleRepository.Object, _logger.Object, mapper,
                _notificationsHandler.Object, _unitOfWork.Object, _MotorcycleProducer.Object, _deliveryExternalService.Object, _systemDate.Object);
        }

        [Fact]
        public async Task Location_Should_Be_Created_Successfully_When_All_Information_Has_Been_Submitted()
        {
            var LocationCommand = _fixture.Create<CreateLocationCommand>();
            var licenseType = new GetLicenseTypeResponse() { Id = Guid.NewGuid(), LicenseType = ELicenseType.AB };
            var plan = new Plan("Plan 1", 7, 30);

            _deliveryExternalService.Setup(x => x.GetLicenseTypeAsync(It.IsAny<Guid>())).ReturnsAsync(licenseType);
            _MotorcycleRepository.Setup(x => x.GetPlanAsync(It.IsAny<Guid>())).ReturnsAsync(plan);
            _MotorcycleRepository.Setup(x => x.MotorcycleExistsAsync(It.IsAny<Guid>())).ReturnsAsync(true);
            _systemDate.Setup(x=>x.Now).Returns(DateTime.Now);

            var response = await _handler.Handle(LocationCommand, CancellationToken.None);

            Assert.NotNull(response);
            _deliveryExternalService.Verify(x => x.GetLicenseTypeAsync(It.IsAny<Guid>()), Times.Once);
            Assert.Equal(DateTime.Now.Date.AddDays(1), response.InitialDate.Date);
            Assert.Equal(DateTime.Now.AddDays(8).Date, response.EndDate.Date);   
            Assert.Equal(ELocationStatus.Active, response.Status);
            _unitOfWork.Verify(x => x.CommitAsync(), Times.Once);
            _MotorcycleRepository.Verify(x => x.AddLocationAsync(It.IsAny<LocationEntity>()), Times.Once);
            _MotorcycleRepository.Verify(x => x.GetPlanAsync(It.IsAny<Guid>()), Times.Once);
            _MotorcycleRepository.Verify(x => x.MotorcycleExistsAsync(It.IsAny<Guid>()), Times.Once);
            _MotorcycleRepository.Verify(x => x.LocationActiveExistsAsync(It.IsAny<Guid>()), Times.Once);
            _systemDate.Verify(x=>x.Now, Times.Once);   
        }

        [Fact]
        public async Task Motorcycle_Should_Not_Be_Created_When_An_Exception_Was_Thrown()
        {
            var LocationCommand = _fixture.Create<CreateLocationCommand>();
            var licenseType = new GetLicenseTypeResponse() { Id = Guid.NewGuid(), LicenseType = ELicenseType.AB };
            var plan = _fixture.Create<Plan>();

            _deliveryExternalService.Setup(x => x.GetLicenseTypeAsync(It.IsAny<Guid>())).Throws(new Exception());
            _MotorcycleRepository.Setup(x => x.GetPlanAsync(It.IsAny<Guid>())).ReturnsAsync(plan);
            _MotorcycleRepository.Setup(x => x.MotorcycleExistsAsync(It.IsAny<Guid>())).ReturnsAsync(true);

            _notificationsHandler
                .Setup(x => x.AddNotification(It.IsAny<string>(), It.IsAny<ENotificationType>(), It.IsAny<object>()))
                .Returns(_notificationsHandler.Object);

            var response = await _handler.Handle(LocationCommand, CancellationToken.None);

            Assert.Null(response);
            _deliveryExternalService.Verify(x => x.GetLicenseTypeAsync(It.IsAny<Guid>()), Times.Once);
            _unitOfWork.Verify(x => x.CommitAsync(), Times.Never);
            _MotorcycleRepository.Verify(x => x.AddLocationAsync(It.IsAny<LocationEntity>()), Times.Never);
            _MotorcycleRepository.Verify(x => x.GetPlanAsync(It.IsAny<Guid>()), Times.Never);
            _MotorcycleRepository.Verify(x => x.MotorcycleExistsAsync(It.IsAny<Guid>()), Times.Never);
            _MotorcycleRepository.Verify(x => x.LocationActiveExistsAsync(It.IsAny<Guid>()), Times.Never);
            _systemDate.Verify(x => x.Now, Times.Never);
        }

        [Fact]
        public async Task Location_Should_Not_Be_Created_When_Deliveryman_Not_Exists()
        {
            var LocationCommand = _fixture.Create<CreateLocationCommand>();
            var plan = _fixture.Create<Plan>();


            _MotorcycleRepository.Setup(x => x.GetPlanAsync(It.IsAny<Guid>())).ReturnsAsync(plan);
            _MotorcycleRepository.Setup(x => x.MotorcycleExistsAsync(It.IsAny<Guid>())).ReturnsAsync(true);
            _notificationsHandler
                .Setup(x => x.AddNotification(It.IsAny<string>(), It.IsAny<ENotificationType>(), It.IsAny<object>()))
                .Returns(_notificationsHandler.Object);

            var response = await _handler.Handle(LocationCommand, CancellationToken.None);

            Assert.Null(response);
            _deliveryExternalService.Verify(x => x.GetLicenseTypeAsync(It.IsAny<Guid>()), Times.Once);
            _unitOfWork.Verify(x => x.CommitAsync(), Times.Never);
            _MotorcycleRepository.Verify(x => x.AddLocationAsync(It.IsAny<LocationEntity>()), Times.Never);
            _MotorcycleRepository.Verify(x => x.GetPlanAsync(It.IsAny<Guid>()), Times.Never);
            _MotorcycleRepository.Verify(x => x.MotorcycleExistsAsync(It.IsAny<Guid>()), Times.Never);
            _MotorcycleRepository.Verify(x => x.LocationActiveExistsAsync(It.IsAny<Guid>()), Times.Never);
            _systemDate.Verify(x => x.Now, Times.Never);
        }

        [Fact]
        public async Task Motorcycle_Should_Not_Be_Created_When_License_Is_Invalid()
        {
            var LocationCommand = _fixture.Create<CreateLocationCommand>();
            var licenseType = new GetLicenseTypeResponse() { Id = Guid.NewGuid(), LicenseType = ELicenseType.B };
            var plan = _fixture.Create<Plan>();

            _deliveryExternalService.Setup(x => x.GetLicenseTypeAsync(It.IsAny<Guid>())).ReturnsAsync(licenseType);
            _MotorcycleRepository.Setup(x => x.GetPlanAsync(It.IsAny<Guid>())).ReturnsAsync(plan);
            _MotorcycleRepository.Setup(x => x.MotorcycleExistsAsync(It.IsAny<Guid>())).ReturnsAsync(true);
            _notificationsHandler
                .Setup(x => x.AddNotification(It.IsAny<string>(), It.IsAny<ENotificationType>(), It.IsAny<object>()))
                .Returns(_notificationsHandler.Object);

            var response = await _handler.Handle(LocationCommand, CancellationToken.None);

            Assert.Null(response);
            _deliveryExternalService.Verify(x => x.GetLicenseTypeAsync(It.IsAny<Guid>()), Times.Once);
            _unitOfWork.Verify(x => x.CommitAsync(), Times.Never);
            _MotorcycleRepository.Verify(x => x.AddLocationAsync(It.IsAny<LocationEntity>()), Times.Never);
            _MotorcycleRepository.Verify(x => x.GetPlanAsync(It.IsAny<Guid>()), Times.Never);
            _MotorcycleRepository.Verify(x => x.MotorcycleExistsAsync(It.IsAny<Guid>()), Times.Never);
            _MotorcycleRepository.Verify(x => x.LocationActiveExistsAsync(It.IsAny<Guid>()), Times.Never);
            _systemDate.Verify(x => x.Now, Times.Never);
        }

        [Fact]
        public async Task Location_Should_Not_Be_Created_When_Plan_Not_Exists()
        {
            var LocationCommand = _fixture.Create<CreateLocationCommand>();
            var licenseType = new GetLicenseTypeResponse() { Id = Guid.NewGuid(), LicenseType = ELicenseType.AB };

            _deliveryExternalService.Setup(x => x.GetLicenseTypeAsync(It.IsAny<Guid>())).ReturnsAsync(licenseType);       
            _MotorcycleRepository.Setup(x => x.MotorcycleExistsAsync(It.IsAny<Guid>())).ReturnsAsync(true);
            _notificationsHandler
                .Setup(x => x.AddNotification(It.IsAny<string>(), It.IsAny<ENotificationType>(), It.IsAny<object>()))
                .Returns(_notificationsHandler.Object);

            var response = await _handler.Handle(LocationCommand, CancellationToken.None);

            Assert.Null(response);
            _deliveryExternalService.Verify(x => x.GetLicenseTypeAsync(It.IsAny<Guid>()), Times.Once);
            _unitOfWork.Verify(x => x.CommitAsync(), Times.Never);
            _MotorcycleRepository.Verify(x => x.AddLocationAsync(It.IsAny<LocationEntity>()), Times.Never);
            _MotorcycleRepository.Verify(x => x.GetPlanAsync(It.IsAny<Guid>()), Times.Once);
            _MotorcycleRepository.Verify(x => x.MotorcycleExistsAsync(It.IsAny<Guid>()), Times.Never);
            _MotorcycleRepository.Verify(x => x.LocationActiveExistsAsync(It.IsAny<Guid>()), Times.Never);
            _systemDate.Verify(x => x.Now, Times.Never);
        }

        [Fact]
        public async Task Location_Should_Not_Be_Created_When_Motorcycle_Not_Exists()
        {
            var LocationCommand = _fixture.Create<CreateLocationCommand>();
            var licenseType = new GetLicenseTypeResponse() { Id = Guid.NewGuid(), LicenseType = ELicenseType.AB };
            var plan = _fixture.Create<Plan>();

            _deliveryExternalService.Setup(x => x.GetLicenseTypeAsync(It.IsAny<Guid>())).ReturnsAsync(licenseType);
            _MotorcycleRepository.Setup(x => x.GetPlanAsync(It.IsAny<Guid>())).ReturnsAsync(plan);
            _MotorcycleRepository.Setup(x => x.MotorcycleExistsAsync(It.IsAny<Guid>())).ReturnsAsync(false);
            _notificationsHandler
                .Setup(x => x.AddNotification(It.IsAny<string>(), It.IsAny<ENotificationType>(), It.IsAny<object>()))
                .Returns(_notificationsHandler.Object);

            var response = await _handler.Handle(LocationCommand, CancellationToken.None);

            Assert.Null(response);
            _deliveryExternalService.Verify(x => x.GetLicenseTypeAsync(It.IsAny<Guid>()), Times.Once);
            _unitOfWork.Verify(x => x.CommitAsync(), Times.Never);
            _MotorcycleRepository.Verify(x => x.AddLocationAsync(It.IsAny<LocationEntity>()), Times.Never);
            _MotorcycleRepository.Verify(x => x.GetPlanAsync(It.IsAny<Guid>()), Times.Once);
            _MotorcycleRepository.Verify(x => x.MotorcycleExistsAsync(It.IsAny<Guid>()), Times.Once);
            _MotorcycleRepository.Verify(x => x.LocationActiveExistsAsync(It.IsAny<Guid>()), Times.Never);
            _systemDate.Verify(x => x.Now, Times.Never);
        }

        [Fact]
        public async Task Location_Should_Not_Be_Created_When_Motorcycle_Has_A_Location_Active()
        {
            var LocationCommand = _fixture.Create<CreateLocationCommand>();
            var licenseType = new GetLicenseTypeResponse() { Id = Guid.NewGuid(), LicenseType = ELicenseType.AB };
            var plan = _fixture.Create<Plan>();

            _deliveryExternalService.Setup(x => x.GetLicenseTypeAsync(It.IsAny<Guid>())).ReturnsAsync(licenseType);
            _MotorcycleRepository.Setup(x => x.GetPlanAsync(It.IsAny<Guid>())).ReturnsAsync(plan);
            _MotorcycleRepository.Setup(x => x.MotorcycleExistsAsync(It.IsAny<Guid>())).ReturnsAsync(true);
            _MotorcycleRepository.Setup(x => x.LocationActiveExistsAsync(It.IsAny<Guid>())).ReturnsAsync(true);
            _notificationsHandler
                .Setup(x => x.AddNotification(It.IsAny<string>(), It.IsAny<ENotificationType>(), It.IsAny<object>()))
                .Returns(_notificationsHandler.Object);

            var response = await _handler.Handle(LocationCommand, CancellationToken.None);

            Assert.Null(response);
            _deliveryExternalService.Verify(x => x.GetLicenseTypeAsync(It.IsAny<Guid>()), Times.Once);
            _unitOfWork.Verify(x => x.CommitAsync(), Times.Never);
            _MotorcycleRepository.Verify(x => x.AddLocationAsync(It.IsAny<LocationEntity>()), Times.Never);
            _MotorcycleRepository.Verify(x => x.GetPlanAsync(It.IsAny<Guid>()), Times.Once);
            _MotorcycleRepository.Verify(x => x.MotorcycleExistsAsync(It.IsAny<Guid>()), Times.Once);
            _MotorcycleRepository.Verify(x => x.LocationActiveExistsAsync(It.IsAny<Guid>()), Times.Once);
            _systemDate.Verify(x => x.Now, Times.Never);
        }
    }
}
