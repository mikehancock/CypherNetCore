namespace CypherNet.Core
{
    using System.Linq;
    using System.Threading.Tasks;

    using Newtonsoft.Json;

    internal class TransactionalNeoRestApiClient : ISendRestCommandsToNeo, ICypherUnitOfWork
    {
        #region Constants

        private const string CommandFormat = @"{{""statements"": [{{""statement"": ""{0}""}}]}};";

        #endregion

        #region Fields

        private readonly IJsonHttpClientWrapper httpClient;

        private readonly string transactionUrl;

        private string commitUrl;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initialises a new instance of the <see cref="TransactionalNeoRestApiClient"/> class.
        /// </summary>
        /// <param name="httpClient">
        /// The http client.
        /// </param>
        /// <param name="transactionUrl">
        /// The transaction url.
        /// </param>
        public TransactionalNeoRestApiClient(IJsonHttpClientWrapper httpClient, string transactionUrl)
        {
            this.httpClient = httpClient;
            this.transactionUrl = transactionUrl;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The commit async.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task CommitAsync()
        {
            if (await this.KeepAliveAsync())
            {
                await this.httpClient.PostAsync(this.commitUrl, string.Empty);
            }
        }

        /// <summary>
        /// The keep alive async.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<bool> KeepAliveAsync()
        {
            var result = await this.httpClient.PostAsync(this.GetThisTransactionUrl(), null);
            var response = JsonConvert.DeserializeObject<NeoResponse>(result);
            return response.errors == null || !response.errors.Any();
        }

        /// <summary>
        /// The rollback async.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task RollbackAsync()
        {
            await this.httpClient.DeleteAsync(this.GetThisTransactionUrl());
        }

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
            var commandUrl = string.IsNullOrEmpty(this.commitUrl) ? this.transactionUrl + "/" : this.GetThisTransactionUrl();

            var result = await this.httpClient.PostAsync(commandUrl, string.Format(CommandFormat, command));

            var response = JsonConvert.DeserializeObject<NeoResponse>(result);

            if (string.IsNullOrEmpty(this.commitUrl))
            {
                this.commitUrl = response.Commit;
            }

            return response;
        }

        #endregion

        #region Methods

        private string GetThisTransactionUrl()
        {
            return this.commitUrl.Substring(0, this.commitUrl.Length - "/commit".Length);
        }

        #endregion
    }
}