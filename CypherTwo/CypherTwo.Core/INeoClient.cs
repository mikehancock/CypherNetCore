namespace CypherTwo.Core
{
    using System;

    using NeoPlayground.Tests;

    using Newtonsoft.Json;

    public interface INeoClient
    {
        void Connect();

        ICypherDataReader Query(string cypher);

        void Execute(string cypher);
    }

    public class NeoClient : INeoClient
    {
        private readonly ISendRestCommandsToNeo neoApi;

        public NeoClient(ISendRestCommandsToNeo neoApi)
        {
            this.neoApi = neoApi;
        }

        public void Connect()
        {
            throw new System.NotImplementedException();
        }

        public ICypherDataReader Query(string cypher)
        {
            var response = this.neoApi.SendCommand(cypher);
            var neoResponse = JsonConvert.DeserializeObject<NeoResponse>(response);
            throw new NotImplementedException();
        }

        public void Execute(string cypher)
        {
            throw new System.NotImplementedException();
        }
    }

}
