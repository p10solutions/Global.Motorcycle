using Microsoft.Extensions.Logging;
using Moq;
using Global.Motorcycle.Domain.Contracts.Notifications;
using Global.Motorcycle.Domain.Contracts.Events;
using Global.Motorcycle.Domain.Contracts.Data.Repositories;
using Global.Motorcycle.Domain.Contracts.Data;
using AutoMapper;
using Global.Motorcycle.Domain.Entities;
using Global.Motorcycle.Application.Features.Locations.Commands.ReturnLease;
using LocationEntity = Global.Motorcycle.Domain.Entities.Location;
using Global.Motorcycle.Domain.Contracts.Date;
using Global.Motorcycle.Domain.Models.Notifications;

namespace Global.Motorcycle.UnitTest.Application.Features.Locations.Commands.ReturnLease
{
    public class ReturnLeaseHandlerUnitTest
    {
        readonly Mock<IMotorcycleRepository> _MotorcycleRepository;
        readonly Mock<ILocationProducer> _MotorcycleProducer;
        readonly Mock<ILogger<ReturnLeaseHandler>> _logger;
        readonly Mock<INotificationsHandler> _notificationsHandler;
        readonly Mock<IUnitOfWork> _unitOfWork;
        readonly Mock<ISystemDate> _systemDate;
        readonly ReturnLeaseHandler _handler;

        public ReturnLeaseHandlerUnitTest()
        {
            _MotorcycleRepository = new Mock<IMotorcycleRepository>();
            _MotorcycleProducer = new Mock<ILocationProducer>();
            _logger = new Mock<ILogger<ReturnLeaseHandler>>();
            _notificationsHandler = new Mock<INotificationsHandler>();
            _unitOfWork = new Mock<IUnitOfWork>();
            _systemDate = new Mock<ISystemDate>();
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new ReturnLeaseMapper());
            });
            IMapper mapper = mappingConfig.CreateMapper();

            _handler = new ReturnLeaseHandler(_MotorcycleRepository.Object, _logger.Object, mapper,
                _notificationsHandler.Object, _unitOfWork.Object, _MotorcycleProducer.Object);
        }

        [Fact]
        public async Task Location_Plan_7_Days_Should_Be_Returned_Successfully_When_All_Information_Has_Been_Submitted()
        {
            var LocationCommand = new ReturnLeaseCommand(Guid.NewGuid(), DateTime.Now);
            var location = new LocationEntity(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
            var plan = new Plan("Plan 1", 7, 30);

            _systemDate
                .Setup(x => x.Now)
                .Returns(DateTime.Now.AddDays(-8));

            location
                .SetInitialDate(_systemDate.Object)
                .SetPlan(plan);

            _MotorcycleRepository
                .Setup(x => x.GetLocationAsync(It.IsAny<Guid>()))
                .ReturnsAsync(location);

            var response = await _handler.Handle(LocationCommand, CancellationToken.None);

            Assert.NotNull(response);
            Assert.Equal(ELocationStatus.Returned, response.Status);
            _unitOfWork.Verify(x => x.CommitAsync(), Times.Once);
            _MotorcycleRepository.Verify(x => x.UpdateLocation(It.IsAny<LocationEntity>()), Times.Once);
            _MotorcycleRepository.Verify(x => x.GetLocationAsync(It.IsAny<Guid>()), Times.Once);
        }

        [Fact]
        public async Task Location_Plan_7_Days_Should_Be_Returned_Successfully_When_Returned_After()
        {
            var LocationCommand = new ReturnLeaseCommand(Guid.NewGuid(), DateTime.Now);
            var location = new LocationEntity(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
            var plan = new Plan("Plan 1", 7, 30) { FeeAfter = 50 };

            _systemDate
                .Setup(x => x.Now)
                .Returns(DateTime.Now.AddDays(-9));

            location
                .SetInitialDate(_systemDate.Object)
                .SetPlan(plan);

            _MotorcycleRepository
                .Setup(x => x.GetLocationAsync(It.IsAny<Guid>()))
                .ReturnsAsync(location);

            var response = await _handler.Handle(LocationCommand, CancellationToken.None);
            var expectedValue = (7 * 30) + 50;

            Assert.NotNull(response);
            Assert.Equal(ELocationStatus.Returned, response.Status);
            _unitOfWork.Verify(x => x.CommitAsync(), Times.Once);
            Assert.Equal(expectedValue, location.Amount);
            _MotorcycleRepository.Verify(x => x.UpdateLocation(It.IsAny<LocationEntity>()), Times.Once);
            _MotorcycleRepository.Verify(x => x.GetLocationAsync(It.IsAny<Guid>()), Times.Once);
        }

        [Fact]
        public async Task Location_Plan_7_Days_Should_Be_Returned_Successfully_When_Returned_Before()
        {
            var LocationCommand = new ReturnLeaseCommand(Guid.NewGuid(), DateTime.Now.AddDays(-2));
            var location = new LocationEntity(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
            var plan = new Plan("Plan 1", 7, 30) { FeeBefore = 20 };

            _systemDate
                .Setup(x => x.Now)
                .Returns(DateTime.Now.AddDays(-8));

            location
                .SetInitialDate(_systemDate.Object)
                .SetPlan(plan);

            _MotorcycleRepository
                .Setup(x => x.GetLocationAsync(It.IsAny<Guid>()))
                .ReturnsAsync(location);

            var response = await _handler.Handle(LocationCommand, CancellationToken.None);

            var expectedValue = (5 * 30) + (0.20 * 2 * 30);

            Assert.NotNull(response);
            Assert.Equal(ELocationStatus.Returned, response.Status);
            _unitOfWork.Verify(x => x.CommitAsync(), Times.Once);
            Assert.Equal(expectedValue, location.Amount);
            _MotorcycleRepository.Verify(x => x.UpdateLocation(It.IsAny<LocationEntity>()), Times.Once);
            _MotorcycleRepository.Verify(x => x.GetLocationAsync(It.IsAny<Guid>()), Times.Once);
        }

        [Fact]
        public async Task Location_Plan_15_Days_Should_Be_Returned_Successfully_When_All_Information_Has_Been_Submitted()
        {
            var LocationCommand = new ReturnLeaseCommand(Guid.NewGuid(), DateTime.Now);
            var location = new LocationEntity(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
            var plan = new Plan("Plan 1", 15, 28);

            _systemDate
                .Setup(x => x.Now)
                .Returns(DateTime.Now.AddDays(-16));

            location
                .SetInitialDate(_systemDate.Object)
                .SetPlan(plan);

            _MotorcycleRepository
                .Setup(x => x.GetLocationAsync(It.IsAny<Guid>()))
                .ReturnsAsync(location);

            var response = await _handler.Handle(LocationCommand, CancellationToken.None);

            Assert.NotNull(response);
            Assert.Equal(ELocationStatus.Returned, response.Status);
            _unitOfWork.Verify(x => x.CommitAsync(), Times.Once);
            _MotorcycleRepository.Verify(x => x.UpdateLocation(It.IsAny<LocationEntity>()), Times.Once);
            _MotorcycleRepository.Verify(x => x.GetLocationAsync(It.IsAny<Guid>()), Times.Once);
        }

        [Fact]
        public async Task Location_Plan_15_Days_Should_Be_Returned_Successfully_When_Returned_Before()
        {
            var LocationCommand = new ReturnLeaseCommand(Guid.NewGuid(), DateTime.Now.AddDays(-5));
            var location = new LocationEntity(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
            var plan = new Plan("Plan 1", 15, 28) { FeeBefore = 40 };

            _systemDate
                .Setup(x => x.Now)
                .Returns(DateTime.Now.AddDays(-16));

            location
                .SetInitialDate(_systemDate.Object)
                .SetPlan(plan);

            _MotorcycleRepository
                .Setup(x => x.GetLocationAsync(It.IsAny<Guid>()))
                .ReturnsAsync(location);

            var response = await _handler.Handle(LocationCommand, CancellationToken.None);

            var expectedValue = (10 * 28) + (0.40 * 5 * 28);

            Assert.NotNull(response);
            Assert.Equal(ELocationStatus.Returned, response.Status);
            Assert.Equal(expectedValue, location.Amount);
            _unitOfWork.Verify(x => x.CommitAsync(), Times.Once);
            _MotorcycleRepository.Verify(x => x.UpdateLocation(It.IsAny<LocationEntity>()), Times.Once);
            _MotorcycleRepository.Verify(x => x.GetLocationAsync(It.IsAny<Guid>()), Times.Once);
        }

        [Fact]
        public async Task Location_Plan_30_Days_Should_Be_Returned_Successfully_When_All_Information_Has_Been_Submitted()
        {
            var LocationCommand = new ReturnLeaseCommand(Guid.NewGuid(), DateTime.Now);
            var location = new LocationEntity(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
            var plan = new Plan("Plan 1", 30, 22);

            _systemDate
                .Setup(x => x.Now)
                .Returns(DateTime.Now.AddDays(-31));

            location
                .SetInitialDate(_systemDate.Object)
                .SetPlan(plan);

            _MotorcycleRepository
                .Setup(x => x.GetLocationAsync(It.IsAny<Guid>()))
                .ReturnsAsync(location);

            var response = await _handler.Handle(LocationCommand, CancellationToken.None);

            Assert.NotNull(response);
            Assert.Equal(ELocationStatus.Returned, response.Status);
            _unitOfWork.Verify(x => x.CommitAsync(), Times.Once);
            _MotorcycleRepository.Verify(x => x.UpdateLocation(It.IsAny<LocationEntity>()), Times.Once);
            _MotorcycleRepository.Verify(x => x.GetLocationAsync(It.IsAny<Guid>()), Times.Once);
        }


        [Fact]
        public async Task Location_Plan_45_Days_Should_Be_Returned_Successfully_When_All_Information_Has_Been_Submitted()
        {
            var LocationCommand = new ReturnLeaseCommand(Guid.NewGuid(), DateTime.Now);
            var location = new LocationEntity(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
            var plan = new Plan("Plan 1", 45, 20);

            _systemDate
                .Setup(x => x.Now)
                .Returns(DateTime.Now.AddDays(-46));

            location
                .SetInitialDate(_systemDate.Object)
                .SetPlan(plan);

            _MotorcycleRepository
                .Setup(x => x.GetLocationAsync(It.IsAny<Guid>()))
                .ReturnsAsync(location);

            var response = await _handler.Handle(LocationCommand, CancellationToken.None);

            Assert.NotNull(response);
            Assert.Equal(ELocationStatus.Returned, response.Status);
            _unitOfWork.Verify(x => x.CommitAsync(), Times.Once);
            _MotorcycleRepository.Verify(x => x.UpdateLocation(It.IsAny<LocationEntity>()), Times.Once);
            _MotorcycleRepository.Verify(x => x.GetLocationAsync(It.IsAny<Guid>()), Times.Once);
        }

        [Fact]
        public async Task Location_Plan_50_Days_Should_Be_Returned_Successfully_When_All_Information_Has_Been_Submitted()
        {
            var LocationCommand = new ReturnLeaseCommand(Guid.NewGuid(), DateTime.Now);
            var location = new LocationEntity(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
            var plan = new Plan("Plan 1", 50, 18);

            _systemDate
                .Setup(x => x.Now)
                .Returns(DateTime.Now.AddDays(-51));

            location
                .SetInitialDate(_systemDate.Object)
                .SetPlan(plan);

            _MotorcycleRepository
                .Setup(x => x.GetLocationAsync(It.IsAny<Guid>()))
                .ReturnsAsync(location);

            var response = await _handler.Handle(LocationCommand, CancellationToken.None);

            Assert.NotNull(response);
            Assert.Equal(ELocationStatus.Returned, response.Status);
            _unitOfWork.Verify(x => x.CommitAsync(), Times.Once);
            _MotorcycleRepository.Verify(x => x.UpdateLocation(It.IsAny<LocationEntity>()), Times.Once);
            _MotorcycleRepository.Verify(x => x.GetLocationAsync(It.IsAny<Guid>()), Times.Once);
        }

        [Fact]
        public async Task Location_Should_Not_Be_Returned_When_An_Exception_Was_Thrown()
        {
            var LocationCommand = new ReturnLeaseCommand(Guid.NewGuid(), DateTime.Now);

            _MotorcycleRepository
                .Setup(x => x.GetLocationAsync(It.IsAny<Guid>()))
                .Throws(new Exception());
            _notificationsHandler
                .Setup(x => x.AddNotification(It.IsAny<string>(), It.IsAny<ENotificationType>(), It.IsAny<object>()))
                .Returns(_notificationsHandler.Object);

            var response = await _handler.Handle(LocationCommand, CancellationToken.None);

            Assert.Null(response);
            _unitOfWork.Verify(x => x.CommitAsync(), Times.Never);
            _MotorcycleRepository.Verify(x => x.UpdateLocation(It.IsAny<LocationEntity>()), Times.Never);
            _MotorcycleRepository.Verify(x => x.GetLocationAsync(It.IsAny<Guid>()), Times.Once);
        }

        [Fact]
        public async Task Location_Should_Not_Be_Returned_When_Location_Not_Exists()
        {
            var LocationCommand = new ReturnLeaseCommand(Guid.NewGuid(), DateTime.Now);
            var location = new LocationEntity(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
            var plan = new Plan("Plan 1", 7, 30);

            _notificationsHandler
                .Setup(x => x.AddNotification(It.IsAny<string>(), It.IsAny<ENotificationType>(), It.IsAny<object>()))
                .Returns(_notificationsHandler.Object);

            var response = await _handler.Handle(LocationCommand, CancellationToken.None);

            Assert.Null(response);
            _unitOfWork.Verify(x => x.CommitAsync(), Times.Never);
            _MotorcycleRepository.Verify(x => x.UpdateLocation(It.IsAny<LocationEntity>()), Times.Never);
            _MotorcycleRepository.Verify(x => x.GetLocationAsync(It.IsAny<Guid>()), Times.Once);
        }

        [Fact]
        public async Task Location_Should_Not_Be_Returned_When_Status_Is_Not_Active()
        {
            var LocationCommand = new ReturnLeaseCommand(Guid.NewGuid(), DateTime.Now);
            var location = new LocationEntity(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
            var plan = new Plan("Plan 1", 50, 18);

            _systemDate
                .Setup(x => x.Now)
                .Returns(DateTime.Now.AddDays(-51));

            location
                .SetInitialDate(_systemDate.Object)
                .SetPlan(plan)
                .GiveBack(DateTime.Now, plan);

            _notificationsHandler
                .Setup(x => x.AddNotification(It.IsAny<string>(), It.IsAny<ENotificationType>(), It.IsAny<object>()))
                .Returns(_notificationsHandler.Object);

            _MotorcycleRepository
                .Setup(x => x.GetLocationAsync(It.IsAny<Guid>()))
                .ReturnsAsync(location);

            var response = await _handler.Handle(LocationCommand, CancellationToken.None);

            Assert.Null(response);
            _unitOfWork.Verify(x => x.CommitAsync(), Times.Never);
            _MotorcycleRepository.Verify(x => x.UpdateLocation(It.IsAny<LocationEntity>()), Times.Never);
            _MotorcycleRepository.Verify(x => x.GetLocationAsync(It.IsAny<Guid>()), Times.Once);
        }

        [Fact]
        public async Task Location_Should_Not_Be_Returned_When_Has_Not_Yet_Started()
        {
            var LocationCommand = new ReturnLeaseCommand(Guid.NewGuid(), DateTime.Now);
            var location = new LocationEntity(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
            var plan = new Plan("Plan 1", 7, 30);

            _systemDate
                .Setup(x => x.Now)
                .Returns(DateTime.Now);

            location
                .SetInitialDate(_systemDate.Object)
                .SetPlan(plan);

            _notificationsHandler
                .Setup(x => x.AddNotification(It.IsAny<string>(), It.IsAny<ENotificationType>(), It.IsAny<object>()))
                .Returns(_notificationsHandler.Object);

            _MotorcycleRepository
                .Setup(x => x.GetLocationAsync(It.IsAny<Guid>()))
                .ReturnsAsync(location);

            var response = await _handler.Handle(LocationCommand, CancellationToken.None);

            Assert.Null(response);
            _unitOfWork.Verify(x => x.CommitAsync(), Times.Never);
            _MotorcycleRepository.Verify(x => x.UpdateLocation(It.IsAny<LocationEntity>()), Times.Never);
            _MotorcycleRepository.Verify(x => x.GetLocationAsync(It.IsAny<Guid>()), Times.Once);
        }
    }
}
