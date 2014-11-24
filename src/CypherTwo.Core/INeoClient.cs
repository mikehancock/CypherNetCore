// --------------------------------------------------------------------------------------------------------------------
// <copyright file="INeoClient.cs" Copyright (c) 2013 Plaza De Armas Ltd>
//   Copyright (c) 2013 Plaza De Armas Ltd
// </copyright>
// <summary>
//   The NeoClient interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CypherTwo.Core
{
    using System.Threading.Tasks;

    /// <summary>
    /// The NeoClient interface.
    /// </summary>
    public interface INeoClient
    {
        #region Public Methods and Operators

        /// <summary>
        /// The execute async.
        /// </summary>
        /// <param name="cypher">
        /// The cypher.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task ExecuteAsync(string cypher);

        /// <summary>
        /// The query async.
        /// </summary>
        /// <param name="cypher">
        /// The cypher.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task<ICypherDataReader> QueryAsync(string cypher);

        #endregion
    }
}