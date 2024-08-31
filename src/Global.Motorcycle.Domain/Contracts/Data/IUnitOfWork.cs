namespace Global.Motorcycle.Domain.Contracts.Data
{
    public interface IUnitOfWork
    {
        Task CommitAsync();
    }
}
