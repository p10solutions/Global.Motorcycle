using Global.Motorcycle.Domain.Contracts.Data;

namespace Global.Motorcycle.Infraestructure.Data
{
    public class UnitOfWork: IUnitOfWork
    {
        readonly MotorcycleManagementContext _context;

        public UnitOfWork(MotorcycleManagementContext context)
        {
            _context = context;
        }

        public async Task CommitAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
