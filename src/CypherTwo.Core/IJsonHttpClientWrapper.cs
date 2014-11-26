namespace CypherNet.Core
{
    using System.Threading.Tasks;

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