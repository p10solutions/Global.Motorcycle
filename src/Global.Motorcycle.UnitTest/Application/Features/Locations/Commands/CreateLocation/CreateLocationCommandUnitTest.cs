using AutoFixture;
using Global.Motorcycle.Application.Features.Locations.Commands.CreateLocation;

namespace Global.Motorcycle.UnitTest.Application.Features.Locations.Commands.CreateLocation
{
    public class CreateLocationCommandUnitTest
    {
        readonly Fixture _fixture;

        public CreateLocationCommandUnitTest()
        {
            _fixture = new Fixture();
        }

        [Fact]
        public void Command_Should_Be_Valid()
        {
            var command = _fixture.Create<CreateLocationCommand>();

            var result = command.Validate();

            Assert.True(result);
        }

        [Fact]
        public void Command_Should_Be_Invalid()
        {
            var command = new CreateLocationCommand(Guid.Empty, Guid.Empty, Guid.Empty);

            var result = command.Validate();

            Assert.False(result);
        }
    }
}
