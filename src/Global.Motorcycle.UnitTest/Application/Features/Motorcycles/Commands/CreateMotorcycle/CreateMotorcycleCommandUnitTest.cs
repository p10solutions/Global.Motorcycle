using AutoFixture;
using Global.Motorcycle.Application.Features.Motorcycles.Commands.CreateMotorcycle;
using Global.Motorcycle.Domain.Entities;

namespace Global.Motorcycle.UnitTest.Application.Features.Motorcycles.Commands.CreateMotorcycle
{
    public class CreateMotorcycleCommandUnitTest
    {
        readonly Fixture _fixture;

        public CreateMotorcycleCommandUnitTest()
        {
            _fixture = new Fixture();
        }

        [Fact]
        public void Command_Should_Be_Valid()
        {
            _fixture.Customize<CreateMotorcycleCommand>(c => c
                .With(p => p.Plate, _fixture.Create<string>().Substring(0, 10)));

            var command = _fixture.Create<CreateMotorcycleCommand>();

            var result = command.Validate();

            Assert.True(result);
        }

        [Fact]
        public void Command_Should_Be_Invalid_When_Model_Not_Informed()
        {
            EMotorcycleStatus status = EMotorcycleStatus.Active;

            var command = new CreateMotorcycleCommand(
                String.Empty,
                "572358",
                2023,
                status
            );

            var result = command.Validate();

            Assert.False(result);
        }

        [Fact]
        public void Command_Should_Be_Invalid_When_Plate_Not_Informed()
        {
            EMotorcycleStatus status = EMotorcycleStatus.Active;

            var command = new CreateMotorcycleCommand(
                "Model",
                String.Empty,
                2023,
                status
            );

            var result = command.Validate();

            Assert.False(result);
        }
    }
}
