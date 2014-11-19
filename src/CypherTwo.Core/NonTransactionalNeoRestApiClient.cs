namespace CypherTwo.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Newtonsoft.Json;

    internal class NonTransactionalNeoRestApiClient : ISendRestCommandsToNeo
    {
        private const string CommandFormat = @"{{""statements"": [{{""statement"": ""{0}""}}]}};";
        private readonly IJsonHttpClientWrapper httpClient;
        private readonly string baseUrl;
        private IDictionary<string, object> serviceRoot;

        public NonTransactionalNeoRestApiClient(IJsonHttpClientWrapper httpClient, string baseUrl)
        {
            this.httpClient = httpClient;
            this.baseUrl = baseUrl;
        }

        public NonTransactionalNeoRestApiClient(IJsonHttpClientWrapper httpClient, IDictionary<string, object> serviceRoot)
        {
            this.httpClient = httpClient;
            this.serviceRoot = serviceRoot;
        }

        public async Task<NeoResponse> SendCommandAsync(string command)
        {
            if (this.serviceRoot == null || !this.serviceRoot.Any())
                throw new InvalidOperationException("you must call connect before anything else cunts!");

            var result = await this.httpClient.PostAsync(this.serviceRoot["transaction"].ToString() + "/commit", string.Format(CommandFormat, command));

            return JsonConvert.DeserializeObject<NeoResponse>(result);
        }

        public async Task LoadServiceRootAsync()
        {
            var result = await this.httpClient.GetAsync(this.baseUrl);
            this.serviceRoot = JsonConvert.DeserializeObject<Dictionary<string, object>>(result);
        }
    }
}