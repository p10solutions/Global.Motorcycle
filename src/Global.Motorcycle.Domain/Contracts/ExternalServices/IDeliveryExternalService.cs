using Global.Motorcycle.Domain.Models.ExternalServices.Delivery.Deliveryman;

namespace Global.Motorcycle.Domain.Contracts.ExternalServices
{
    public interface IDeliveryExternalService
    {
        Task<GetLicenseTypeResponse?> GetLicenseTypeAsync(Guid deliverymanId);
    }
}
