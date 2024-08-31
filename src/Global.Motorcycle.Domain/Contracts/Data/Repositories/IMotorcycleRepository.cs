using Global.Motorcycle.Domain.Entities;

namespace Global.Motorcycle.Domain.Contracts.Data.Repositories
{
    public interface IMotorcycleRepository
    {
        Task AddAsync(MotorcycleEntity Motorcycle);
        void Update(MotorcycleEntity Motorcycle);
        void UpdatePlate(Guid motorcycleId, string plate);
        Task<MotorcycleEntity?> GetAsync(Guid id);
        Task<IEnumerable<MotorcycleEntity>> GetAsync(string? plate);
        Task<bool> ModelExistsAsync(string model, Guid MotorcycleId);
        Task<bool> PlateExistsAsync(string plate, Guid motorcycleId);
        void Delete(Guid id);
        Task AddLocationAsync(Location location);
        Task<bool> LocationActiveExistsAsync(Guid motorcycleId);
        Task<Plan?> GetPlanAsync(Guid planId);
        Task<bool> MotorcycleExistsAsync(Guid id);
        Task<Location?> GetLocationAsync(Guid id);
        void UpdateLocation(Location location);
    }
}
