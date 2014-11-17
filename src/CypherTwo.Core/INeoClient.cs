namespace CypherTwo.Core
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Newtonsoft.Json;

    public interface INeoClient
    {
        void Initialise();

        Task<ICypherDataReader> QueryAsync(string cypher);

        void Execute(string cypher);
    }

    public class NeoClient : INeoClient
    {
        private readonly ISendRestCommandsToNeo neoApi;

        public NeoClient(ISendRestCommandsToNeo neoApi)
        {
            this.neoApi = neoApi;
        }

        public void Initialise()
        {
            try
            {
                var task = this.neoApi.LoadServiceRootAsync();
                task.Wait();
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

        public void Execute(string cypher)
        {
            throw new System.NotImplementedException();
        }
    }
}