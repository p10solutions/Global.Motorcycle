using AutoFixture;
using Global.Motorcycle.Application.Features.Motorcycles.Commands.CreateMotorcycle;
using Global.Motorcycle.Application.Features.Motorcycles.Commands.UpdateMotorcycle;
using Global.Motorcycle.Application.Features.Motorycycles.Commands.UpdateMotorcyclePlate;
using Global.Motorcycle.Domain.Entities;

namespace Global.Motorcycle.UnitTest.Application.Features.Motorcycles.Commands.UpdateMotorcycle
{
    public class UpdateMotorcycleCommandUnitTest
    {
        readonly Fixture _fixture;

        public UpdateMotorcycleCommandUnitTest()
        {
            _fixture = new Fixture();
        }

        [Fact]
        public void Command_Should_Be_Valid()
        {
            _fixture.Customize<UpdateMotorcycleCommand>(c => c
                .With(p => p.Plate, _fixture.Create<string>().Substring(0, 10)));

            var command = _fixture.Create<UpdateMotorcycleCommand>();

            var result = command.Validate();

            Assert.True(result);
        }

        [Fact]
        public void Command_Should_Be_Invalid()
        {
            EMotorcycleStatus status = EMotorcycleStatus.Active;
            Guid id = Guid.Empty;

            var command = new UpdateMotorcycleCommand(
                id,
                "Yamaha 500",
                "54712",
                2024,
                status
            );

            var result = command.Validate();

            Assert.False(result);
        }
    }
}
