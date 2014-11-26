namespace CypherNet.Core
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// The neo client.
    /// </summary>
    public class NeoClient : INeoClient
    {
        #region Fields

        private readonly ApiClientFactory restCommandFactory;

        #endregion

        #region Constructors and Destructors

        internal NeoClient(NeoDataRootResponse baseUrl)
        {
            this.restCommandFactory = new ApiClientFactory(baseUrl, new JsonHttpClientWrapper());
        }

        #endregion

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
        public async Task ExecuteAsync(string cypher)
        {
            await this.ExecuteCore(cypher);
        }

        /// <summary>
        /// The query async.
        /// </summary>
        /// <param name="cypher">
        /// The cypher.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<ICypherDataReader> QueryAsync(string cypher)
        {
            var neoResponse = await this.ExecuteCore(cypher);

            return new CypherDataReader(neoResponse);
        }

        #endregion

        #region Methods

        private async Task<NeoResponse> ExecuteCore(string cypher)
        {
            var neoResponse = await this.restCommandFactory.GetApiClient().SendCommandAsync(cypher);
            if (neoResponse.errors != null && neoResponse.errors.Any())
            {
                throw new Exception(string.Join(Environment.NewLine, neoResponse.errors.Select(error => error.ToObject<string>())));
            }

            return neoResponse;
        }

        #endregion
    }
}