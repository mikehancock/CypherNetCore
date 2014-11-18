namespace CypherTwo.Core
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Newtonsoft.Json;

    public interface INeoClient
    {
        Task InitialiseAsync();

        Task<ICypherDataReader> QueryAsync(string cypher);

        Task ExecuteAsync(string cypher);
    }

    public class NeoClient : INeoClient
    {
        private readonly ISendRestCommandsToNeo neoApi;

        public NeoClient(ISendRestCommandsToNeo neoApi)
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
            var response = await this.neoApi.SendCommandAsync(cypher);
            var neoResponse = JsonConvert.DeserializeObject<NeoResponse>(response);
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