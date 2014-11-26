namespace CypherNet.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// The neo client.
    /// </summary>
    public class NeoClient : INeoClient
    {
        #region Fields

        private readonly IApiClientFactory restCommandFactory;

        #endregion

        #region Constructors and Destructors

        internal NeoClient(IApiClientFactory restCommandFactory)
        {
            this.restCommandFactory = restCommandFactory;
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
                throw new Exception(string.Join(Environment.NewLine, neoResponse.errors.Select(error => error.ToObject<NeoError>())));
            }

            return neoResponse;
        }

        #endregion

        internal class NeoError
        {
            public string Code { get; set; }
            public string Message { get; set; }

            public override string ToString()
            {
                return this.Code + ": " + this.Message;
            }
        }
    }
}