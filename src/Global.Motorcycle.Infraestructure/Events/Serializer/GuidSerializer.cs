using Confluent.Kafka;
using System.Text.Json;
using System.Text;

namespace Global.Motorcycle.Infraestructure.Events.Serializer
{
    public class GuidSerializer : IAsyncSerializer<Guid>
    {
        public Task<byte[]> SerializeAsync(Guid data, SerializationContext context)
        {
            var json = JsonSerializer.Serialize(data);
            return Task.FromResult(Encoding.ASCII.GetBytes(json));
        }
    }
}
