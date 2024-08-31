using Global.Motorcycle.Domain.Contracts.Data.Repositories;
using Global.Motorcycle.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Global.Motorcycle.Infraestructure.Data.Repositories
{
    public class MotorcycleRepository : IMotorcycleRepository
    {
        readonly MotorcycleManagementContext _context;

        public MotorcycleRepository(MotorcycleManagementContext context)
        {
            _context = context;
        }

        public async Task AddAsync(MotorcycleEntity Motorcycle)
            => await _context.Motorcycle.AddAsync(Motorcycle);

        public async Task<MotorcycleEntity?> GetAsync(Guid id)
            => await _context.Motorcycle
                .SingleOrDefaultAsync(x => x.Id == id);

        public void Update(MotorcycleEntity Motorcycle)
            => _context.Motorcycle.Update(Motorcycle);

        public async Task<bool> ModelExistsAsync(string model,  Guid MotorcycleId)
            => await _context.Motorcycle.AnyAsync(x => x.Model == model && x.Model == model && x.Id != MotorcycleId);

        public async Task<bool> PlateExistsAsync(string plate, Guid motorcycleId)
            => await _context.Motorcycle.AnyAsync(x => x.Plate == plate && x.Id != motorcycleId);

        public async Task<bool> MotorcycleExistsAsync(Guid id)
            => await _context.Motorcycle.AnyAsync(x => x.Id == id);

        public async Task<IEnumerable<MotorcycleEntity>> GetAsync(string? plate)
            => await _context.Motorcycle.Where(x => plate == null || x.Plate == plate).ToListAsync();

        public async Task AddLocationAsync(Location location)
            => await _context.Location.AddAsync(location);

        public async Task<bool> LocationActiveExistsAsync(Guid motorcycleId)
            => await _context.Location.AnyAsync(x => x.MotorcycleId == motorcycleId && x.Status == ELocationStatus.Active);

        public void UpdatePlate(Guid motorcycleId, string plate)
        {
            var motorcycle = new MotorcycleEntity() { Id = motorcycleId };

            _context.Motorcycle.Attach(motorcycle);

            motorcycle.Plate = plate;

            _context.Entry(motorcycle).Property(x => x.Plate).IsModified = true;
        }

        public void Delete(Guid id)
        {
            var motorcycle = new MotorcycleEntity() { Id = id };

            _context.Motorcycle.Attach(motorcycle);

            _context.Motorcycle.Remove(motorcycle);
        }

        public async Task<Plan?> GetPlanAsync(Guid planId)
            => await _context.Plan.SingleOrDefaultAsync(x => x.Id == planId);

        public async Task<Location?> GetLocationAsync(Guid id)
            => await _context.Location
                .Include(x => x.Motorcycle)
                .Include(x => x.Plan)
                .SingleOrDefaultAsync(x => x.Id == id);

        public void UpdateLocation(Location location)
            => _context.Location.Update(location);
    }
}
