using Global.Motorcycle.Application.Features.Locations.Commands.ReturnLease;

namespace Global.Motorcycle.UnitTest.Application.Features.Locations.Commands.ReturnLease
{
    public class ReturnLeaseCommandUnitTest
    {
        [Fact]
        public void Command_Should_Be_Valid()
        {
            var command = new ReturnLeaseCommand(Guid.NewGuid(), DateTime.Now);

            var result = command.Validate();

            Assert.True(result);
        }

        [Fact]
        public void Command_Should_Be_Invalid_When_Id_Is_Empty()
        {
            var command = new ReturnLeaseCommand(Guid.Empty, DateTime.Now);

            var result = command.Validate();

            Assert.False(result);
        }

        [Fact]
        public void Command_Should_Be_Invalid_When_Date_Is_Greater_Than_Now()
        {
            var command = new ReturnLeaseCommand(Guid.NewGuid(), DateTime.Now.AddDays(1));

            var result = command.Validate();

            Assert.False(result);
        }
    }
}
