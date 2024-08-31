using Confluent.Kafka;
using Global.Motorcycle.Domain.Models.Events.Locations;
using System.Text;
using System.Text.Json;

namespace Global.Motorcycle.Infraestructure.Events.Locations.Serializers
{
    public class ReturnedLeaseEventSerializer : IAsyncSerializer<ReturnedLeaseEvent>
    {
        public Task<byte[]> SerializeAsync(ReturnedLeaseEvent data, SerializationContext context)
        {
            var json = JsonSerializer.Serialize(data);
            return Task.FromResult(Encoding.ASCII.GetBytes(json));
        }
    }
}
