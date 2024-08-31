using Global.Motorcycle.Domain.Contracts.ExternalServices;
using Global.Motorcycle.Domain.Models.ExternalServices.Delivery.Deliveryman;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace Global.Motorcycle.Infraestructure.ExternalServices
{
    public class DeliveryExternalService : IDeliveryExternalService
    {
        readonly HttpClient _httpClient;

        public DeliveryExternalService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<GetLicenseTypeResponse?> GetLicenseTypeAsync(Guid deliverymanId)
        {
            var serializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter() }
            };

            return await _httpClient.GetFromJsonAsync<GetLicenseTypeResponse>($"deliveryman/{deliverymanId}/licensetype", serializerOptions);
        }
    }
}
