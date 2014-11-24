namespace CypherTwo.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Newtonsoft.Json;

    internal class TransactionalNeoRestApiClient : ISendRestCommandsToNeo, ICypherUnitOfWork
    {
        private const string CommandFormat = @"{{""statements"": [{{""statement"": ""{0}""}}]}};";
        private readonly IJsonHttpClientWrapper httpClient;
        private readonly string transactionUrl;
        private string commitUrl;

        public TransactionalNeoRestApiClient(IJsonHttpClientWrapper httpClient, string transactionUrl)
        {
            this.httpClient = httpClient;
            this.transactionUrl = transactionUrl;
        }

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

        public async Task CommitAsync()
        {
            if (await this.KeepAliveAsync())
            {
                await this.httpClient.PostAsync(this.commitUrl, string.Empty);
            }
        }

        public async Task RollbackAsync()
        {
            await this.httpClient.DeleteAsync(this.GetThisTransactionUrl());
        }

        public async Task<bool> KeepAliveAsync()
        {
            var result = await this.httpClient.PostAsync(this.GetThisTransactionUrl(), null);
            var response = JsonConvert.DeserializeObject<NeoResponse>(result);
            return response.errors == null || !response.errors.Any();
        }

        private string GetThisTransactionUrl()
        {
            return this.commitUrl.Substring(0, this.commitUrl.Length - "/commit".Length);
        }
    }
}