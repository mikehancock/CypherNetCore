namespace CypherTwo.Tests
{
    using CypherTwo.Core;

    using FakeItEasy;

    using NUnit.Framework;

    [TestFixture]
    public class NeoRestApiClientTests
    {
        private ISendRestCommandsToNeo neoRestApiClient;

        private IJsonHttpClientWrapper jsonClientWrapper;

        [SetUp]
        public void SetupBeforeEachTest()
        {
            this.jsonClientWrapper = A.Fake<IJsonHttpClientWrapper>();
            this.neoRestApiClient = new NeoRestApiClient(this.jsonClientWrapper, "http://localhost:7474/");
        }

    }
}
