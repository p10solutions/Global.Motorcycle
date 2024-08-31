using Confluent.Kafka;
using Global.Motorcycle.Domain.Contracts.Events;
using Global.Motorcycle.Domain.Models.Events.Motorcycles;
using Global.Motorcycle.Infraestructure.Events.Motorcycles.Serializers;
using Global.Motorcycle.Infraestructure.Events.Serializer;
using Microsoft.Extensions.Configuration;

namespace Global.Motorcycle.Infraestructure.Events.Motorcycles
{
    public class MotorcycleProducer : IMotorcycleProducer
    {
        readonly ProducerConfig _config;
        readonly string _createTopic;
        readonly string _updateTopic;
        readonly string _deleteTopic;

        public MotorcycleProducer(IConfiguration configuration)
        {
            _createTopic = configuration.GetSection("Kafka:Motorcycle:CreateTopic").Value;
            _updateTopic = configuration.GetSection("Kafka:Motorcycle:UpdateTopic").Value;
            _deleteTopic = configuration.GetSection("Kafka:Motorcycle:DeleteTopic").Value;
            _config = new ProducerConfig
            {
                BootstrapServers = configuration.GetSection("Kafka:Server").Value,
            };
        }

        public async Task SendCreatedEventAsync(CreatedMotorcycleEvent @event)
        {
            using var producer = new ProducerBuilder<Guid, CreatedMotorcycleEvent>(_config)
                .SetKeySerializer(new GuidSerializer())
                .SetValueSerializer(new CreatedMotorcycleEventSerializer())
                .Build();

            var message = new Message<Guid, CreatedMotorcycleEvent>() { Key = @event.Id, Value = @event };

            await producer.ProduceAsync(_createTopic, message);
        }

        public async Task SendUpdatedEventAsync(UpdatedMotorcycleEvent @event)
        {
            using var producer = new ProducerBuilder<Guid, UpdatedMotorcycleEvent>(_config)
                .SetKeySerializer(new GuidSerializer())
                .SetValueSerializer(new UpdatedMotorcycleEventSerializer())
                .Build();

            var message = new Message<Guid, UpdatedMotorcycleEvent>() { Key = @event.Id, Value = @event };

            await producer.ProduceAsync(_updateTopic, message);
        }

        public async Task SendDeletedEventAsync(DeletedMotorcycleEvent @event)
        {
            using var producer = new ProducerBuilder<Guid, DeletedMotorcycleEvent>(_config)
                .SetKeySerializer(new GuidSerializer())
                .SetValueSerializer(new DeletedMotorcycleEventSerializer())
                .Build();

            var message = new Message<Guid, DeletedMotorcycleEvent>() { Key = @event.Id, Value = @event };

            await producer.ProduceAsync(_deleteTopic, message);
        }
    }
}
