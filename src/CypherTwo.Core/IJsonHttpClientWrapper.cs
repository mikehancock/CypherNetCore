// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IJsonHttpClientWrapper.cs" Copyright (c) 2013 Plaza De Armas Ltd>
//   Copyright (c) 2013 Plaza De Armas Ltd
// </copyright>
// <summary>
//   The JsonHttpClientWrapper interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CypherTwo.Core
{
    using System.Threading.Tasks;

    /// <summary>
    /// The JsonHttpClientWrapper interface.
    /// </summary>
    public interface IJsonHttpClientWrapper
    {
        #region Public Methods and Operators

        /// <summary>
        /// The delete async.
        /// </summary>
        /// <param name="url">
        /// The url.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task<string> DeleteAsync(string url);

        /// <summary>
        /// The get async.
        /// </summary>
        /// <param name="url">
        /// The url.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task<string> GetAsync(string url);

        /// <summary>
        /// The post async.
        /// </summary>
        /// <param name="url">
        /// The url.
        /// </param>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task<string> PostAsync(string url, string request);

        #endregion
    }
}