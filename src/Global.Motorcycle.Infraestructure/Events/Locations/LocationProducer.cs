using Confluent.Kafka;
using Global.Motorcycle.Domain.Contracts.Events;
using Global.Motorcycle.Domain.Models.Events.Locations;
using Global.Motorcycle.Infraestructure.Events.Locations.Serializers;
using Global.Motorcycle.Infraestructure.Events.Serializer;
using Microsoft.Extensions.Configuration;

namespace Global.Motorcycle.Infraestructure.Events.Locations
{
    public class LocationProducer : ILocationProducer
    {
        readonly ProducerConfig _config;
        readonly string _createTopic;
        readonly string _returnedTopic;

        public LocationProducer(IConfiguration configuration)
        {
            _createTopic = configuration.GetSection("Kafka:Location:CreateTopic").Value;
            _returnedTopic = configuration.GetSection("Kafka:Location:ReturnedTopic").Value;
            _config = new ProducerConfig
            {
                BootstrapServers = configuration.GetSection("Kafka:Server").Value,
            };
        }

        public async Task SendCreatedEventAsync(CreatedLocationEvent @event)
        {
            using var producer = new ProducerBuilder<Guid, CreatedLocationEvent>(_config)
                .SetKeySerializer(new GuidSerializer())
                .SetValueSerializer(new CreatedLocationEventSerializer())
                .Build();

            var message = new Message<Guid, CreatedLocationEvent>() { Key = @event.Id, Value = @event };

            await producer.ProduceAsync(_createTopic, message);
        }

        public async Task SendReturnedLeaseEventAsync(ReturnedLeaseEvent @event)
        {
            using var producer = new ProducerBuilder<Guid, ReturnedLeaseEvent>(_config)
                .SetKeySerializer(new GuidSerializer())
                .SetValueSerializer(new ReturnedLeaseEventSerializer())
                .Build();

            var message = new Message<Guid, ReturnedLeaseEvent>() { Key = @event.Id, Value = @event };

            await producer.ProduceAsync(_returnedTopic, message);
        }
    }
}
