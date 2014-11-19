namespace CypherTwo.Core
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    public class NeoClient : INeoClient
    {
        private readonly ISendRestCommandsToNeo neoApi;

        public NeoClient(string baseUrl) : this(new ApiClientFactory(baseUrl, new JsonHttpClientWrapper()))
        {
        }

        internal NeoClient(ISendRestCommandsToNeo neoApi)
        {
            this.neoApi = neoApi;
        }

        public async Task InitialiseAsync()
        {
            try
            {
                await this.neoApi.LoadServiceRootAsync();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }

        public async Task<ICypherDataReader> QueryAsync(string cypher)
        {
            var neoResponse = await this.neoApi.SendCommandAsync(cypher);
            if (neoResponse.errors != null && neoResponse.errors.Any())
            {
                throw new Exception(string.Join(Environment.NewLine, neoResponse.errors.Select(error => error.ToObject<string>())));
            }

            return new CypherDataReader(neoResponse);
        }

        public Task ExecuteAsync(string cypher)
        {
            throw new System.NotImplementedException();
        }
    }
}