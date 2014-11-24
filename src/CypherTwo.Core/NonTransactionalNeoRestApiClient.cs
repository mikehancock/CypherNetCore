// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NonTransactionalNeoRestApiClient.cs" Copyright (c) 2013 Plaza De Armas Ltd>
//   Copyright (c) 2013 Plaza De Armas Ltd
// </copyright>
// <summary>
//   Defines the NonTransactionalNeoRestApiClient type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CypherTwo.Core
{
    using System.Threading.Tasks;

    using Newtonsoft.Json;

    internal class NonTransactionalNeoRestApiClient : ISendRestCommandsToNeo
    {
        #region Constants

        private const string CommandFormat = @"{{""statements"": [{{""statement"": ""{0}""}}]}};";

        #endregion

        #region Fields

        private readonly IJsonHttpClientWrapper httpClient;

        private readonly string transactionUrl;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initialises a new instance of the <see cref="NonTransactionalNeoRestApiClient"/> class.
        /// </summary>
        /// <param name="httpClient">
        /// The http client.
        /// </param>
        /// <param name="transactionUrl">
        /// The transaction url.
        /// </param>
        public NonTransactionalNeoRestApiClient(IJsonHttpClientWrapper httpClient, string transactionUrl)
        {
            this.httpClient = httpClient;
            this.transactionUrl = transactionUrl;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The send command async.
        /// </summary>
        /// <param name="command">
        /// The command.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<NeoResponse> SendCommandAsync(string command)
        {
            var result = await this.httpClient.PostAsync(this.transactionUrl + "/commit", string.Format(CommandFormat, command));
            return JsonConvert.DeserializeObject<NeoResponse>(result);
        }

        #endregion
    }
}