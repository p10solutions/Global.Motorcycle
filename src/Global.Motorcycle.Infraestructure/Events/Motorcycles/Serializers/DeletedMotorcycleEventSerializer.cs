using Confluent.Kafka;
using Global.Motorcycle.Domain.Models.Events.Motorcycles;
using System.Text;
using System.Text.Json;

namespace Global.Motorcycle.Infraestructure.Events.Motorcycles.Serializers
{
    public class DeletedMotorcycleEventSerializer : IAsyncSerializer<DeletedMotorcycleEvent>
    {
        public Task<byte[]> SerializeAsync(DeletedMotorcycleEvent data, SerializationContext context)
        {
            var json = JsonSerializer.Serialize(data);
            return Task.FromResult(Encoding.ASCII.GetBytes(json));
        }
    }
}
