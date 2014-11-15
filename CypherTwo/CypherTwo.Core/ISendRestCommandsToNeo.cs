namespace CypherTwo.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Newtonsoft.Json;

    public interface ISendRestCommandsToNeo
    {
        Task<string> SendCommandAsync(string command);

        Task LoadServiceRootAsync();
    }

    public class NeoRestApiClient : ISendRestCommandsToNeo
    {
        private readonly IJsonHttpClientWrapper httpClient;

        private readonly string baseUrl;

        private const string CommandFormat = @"{{""statements"": [{{""statement"": ""{0}""}}]}};";

        private IDictionary<string, object> serviceRoot;

        public NeoRestApiClient(IJsonHttpClientWrapper httpClient, string baseUrl)
        {
            this.httpClient = httpClient;
            this.baseUrl = baseUrl;
        }

        public async Task<string> SendCommandAsync(string command)
        {
            if (this.serviceRoot == null || !this.serviceRoot.Any())
                throw new InvalidOperationException("you must call connect before anything else cunts!");

            var result = await this.httpClient.PostAsync(this.serviceRoot["transaction"].ToString() + "/commit" , string.Format(CommandFormat, command));

            return result;
        }

        public async Task LoadServiceRootAsync()
        {
            var result = await this.httpClient.GetAsync(this.baseUrl);
            this.serviceRoot = JsonConvert.DeserializeObject<Dictionary<string, object>>(result);
        }
    }
}
