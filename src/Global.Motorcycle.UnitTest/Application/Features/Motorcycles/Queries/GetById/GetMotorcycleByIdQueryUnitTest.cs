using AutoFixture;
using Global.Motorcycle.Application.Features.Motorycycles.Queries.GetMotorcycleById;

namespace Global.Motorcycle.UnitTest.Application.Features.Motorcycles.Queries.GetById
{
    public class GetMotorcycleByIdQueryUnitTest
    {
        readonly Fixture _fixture;

        public GetMotorcycleByIdQueryUnitTest()
        {
            _fixture = new Fixture();
        }

        [Fact]
        public void Command_Should_Be_Valid()
        {
            var command = _fixture.Create<GetMotorcycleByIdQuery>();

            var result = command.Validate();

            Assert.True(result);
        }

        [Fact]
        public void Command_Should_Be_Invalid()
        {
            var command = new GetMotorcycleByIdQuery(Guid.Empty);

            var result = command.Validate();

            Assert.False(result);
        }
    }
}
