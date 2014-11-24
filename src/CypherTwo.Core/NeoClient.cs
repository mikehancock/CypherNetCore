using System.Collections.Generic;
using Newtonsoft.Json;

namespace CypherTwo.Core
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;


    public class GraphStore
    {
        private string baseUrl;
        private IJsonHttpClientWrapper httpClient;
        private NeoRootResponse serviceRoot;
        private NeoDataRootResponse dataRoot;

        public GraphStore(string baseUrl) : this(baseUrl, new JsonHttpClientWrapper())
        {
        }

        internal GraphStore(string baseUrl, IJsonHttpClientWrapper httpClient)
        {
            this.baseUrl = baseUrl;
            this.httpClient = httpClient;
        }

        public void Initialize()
        {
            var result = this.httpClient.GetAsync(this.baseUrl).Result;
            this.serviceRoot = JsonConvert.DeserializeObject<NeoRootResponse>(result);
            var dataRootResult = this.httpClient.GetAsync(this.serviceRoot.Data).Result;
            this.dataRoot = JsonConvert.DeserializeObject<NeoDataRootResponse>(dataRootResult);
        }

        public INeoClient GetSession()
        {
          return new NeoClient(this.dataRoot);  
        }
    }

    public class NeoClient : INeoClient
    {
        private readonly ApiClientFactory restCommandFactory;

        internal NeoClient(NeoDataRootResponse baseUrl)
        {
            this.restCommandFactory = new ApiClientFactory(baseUrl, new JsonHttpClientWrapper());
        }

        public async Task<ICypherDataReader> QueryAsync(string cypher)
        {
            var neoResponse = await this.ExecuteCore(cypher);

            return new CypherDataReader(neoResponse);
        }

        public async Task ExecuteAsync(string cypher)
        {
            await this.ExecuteCore(cypher);
        }

        private async Task<NeoResponse> ExecuteCore(string cypher)
        {
            var neoResponse = await this.restCommandFactory.GetApiClient().SendCommandAsync(cypher);
            if (neoResponse.errors != null && neoResponse.errors.Any())
            {
                throw new Exception(string.Join(Environment.NewLine, neoResponse.errors.Select(error => error.ToObject<string>())));
            }

            return neoResponse;
        }
    }
}