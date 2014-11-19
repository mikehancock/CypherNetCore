namespace CypherTwo.Core
{
    using System.Threading.Tasks;

    internal interface ICypherUnitOfWork
    {
        Task CommitAsync();
        Task RollbackAsync();
        Task<bool> KeepAliveAsync();
    }
}