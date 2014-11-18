namespace CypherTwo.Tests
{
    using System;
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
            this.neoApi = new NeoRestApiClient(this.httpClientWrapper, "http://localhost:7474/db/data");
            this.neoClient = new NeoClient(this.neoApi);
            this.neoClient.Initialise();
        }

        [Test]
        public void InitialiseThrowsExecptionWithInvalidUrl()
        {
            this.neoApi = new NeoRestApiClient(this.httpClientWrapper, "http://localhost:1111/");
            this.neoClient = new NeoClient(this.neoApi);
            Assert.Throws<InvalidOperationException>(() => this.neoClient.Initialise());
        }

        [Test]
        public async void CreateAndSelectNode()
        {
            var reader = await this.neoClient.QueryAsync("CREATE (n:Person  { name : 'Andres', title : 'Developer' }) RETURN {Name: n.name, Title: n.title, Id: Id(n)} as TestFoo");
            
            Assert.That(reader.Read(), Is.EqualTo(true));
            var foo = reader.Get<TestFoo>(0);
            Assert.That(foo.Name, Is.EqualTo("Andres"));
            Assert.That(foo.Title, Is.EqualTo("Developer"));
            Assert.That(foo.Id, Is.GreaterThan(-1));
        }

        private class TestFoo
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Title { get; set; }
        }
    }
}