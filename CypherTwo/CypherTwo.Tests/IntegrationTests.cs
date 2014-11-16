namespace CypherTwo.Tests
{
    using System.Net.Http;

    using CypherTwo.Core;

    using NUnit.Framework;

    [TestFixture]
    public class IntegrationTests
    {
        private INeoClient neoClient;

        private ISendRestCommandsToNeo neoApi;

        private IJsonHttpClientWrapper httpClientWrapper;

        [SetUp]
        public void SetupBeforeEachTest()
        {
            this.httpClientWrapper = new JsonHttpClientWrapper(new HttpClient());
            this.neoApi = new NeoRestApiClient(this.httpClientWrapper, "http://localhost:7474/");
            this.neoClient = new NeoClient(this.neoApi);
        }

        [Test]
        public void GetPersonNode()
        {
            this.neoClient.Initialise();
        }
    }
}