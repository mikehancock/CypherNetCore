namespace CypherTwo.Core
{
    using System.Threading.Tasks;
    using Newtonsoft.Json;

    internal class NonTransactionalNeoRestApiClient : ISendRestCommandsToNeo
    {
        private const string CommandFormat = @"{{""statements"": [{{""statement"": ""{0}""}}]}};";
        private readonly IJsonHttpClientWrapper httpClient;
        private readonly string transactionUrl;

        public NonTransactionalNeoRestApiClient(IJsonHttpClientWrapper httpClient, string transactionUrl)
        {
            this.httpClient = httpClient;
            this.transactionUrl = transactionUrl;
        }
       
        public async Task<NeoResponse> SendCommandAsync(string command)
        {
            var result = await this.httpClient.PostAsync(transactionUrl + "/commit", string.Format(CommandFormat, command));
            return JsonConvert.DeserializeObject<NeoResponse>(result);
        }

    }
}