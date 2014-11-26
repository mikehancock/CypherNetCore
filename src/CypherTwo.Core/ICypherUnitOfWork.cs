namespace CypherNet.Core
{
    using System.Threading.Tasks;

    internal interface ICypherUnitOfWork
    {
        #region Public Methods and Operators

        /// <summary>
        /// The commit async.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task CommitAsync();

        /// <summary>
        /// The keep alive async.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task<bool> KeepAliveAsync();

        /// <summary>
        /// The rollback async.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task RollbackAsync();

        #endregion
    }
}