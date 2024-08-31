using Global.Motorcycle.Domain.Entities;

namespace Global.Motorcycle.Domain.Contracts.Cache
{
    public interface IMotorcycleCache
    {
        Task AddAsync(MotorcycleEntity Motorcycle);
        Task<MotorcycleEntity?> GetAsync(Guid id);
    }
}
