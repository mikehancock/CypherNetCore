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
            var neoResponse = await this.ExecuteCore(cypher);

            return new CypherDataReader(neoResponse);
        }

        public async Task ExecuteAsync(string cypher)
        {
            await this.ExecuteCore(cypher);
        }

        private async Task<NeoResponse> ExecuteCore(string cypher)
        {
            var neoResponse = await this.neoApi.SendCommandAsync(cypher);
            if (neoResponse.errors != null && neoResponse.errors.Any())
            {
                throw new Exception(string.Join(Environment.NewLine, neoResponse.errors.Select(error => error.ToObject<string>())));
            }

            return neoResponse;
        }
    }
}