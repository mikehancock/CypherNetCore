namespace CypherTwo.Tests
{
    using System;
    using System.Net.Http;
    using CypherTwo.Core;
    using NUnit.Framework;

    [TestFixture]
    public class IntegrationTests
    {
        private GraphStore graphStore;
        private INeoClient neoClient;
        private IJsonHttpClientWrapper httpClientWrapper;

        [TestFixtureSetUp]
        public void SetupOnce()
        {
            this.graphStore = new GraphStore("http://localhost:7474/", new JsonHttpClientWrapper());
            this.graphStore.Initialize();
        }

        [SetUp]
        public void SetupBeforeEachTest()
        {
            this.httpClientWrapper = new JsonHttpClientWrapper();
            this.neoClient = this.graphStore.GetClient();
        }

        [Test]
        public async void CreateAndSelectNodeUsingQuery()
        {
            var reader = await this.neoClient.QueryAsync("CREATE (n:Person  { name : 'Andres', title : 'Developer' }) RETURN {Name: n.name, Title: n.title, Id: Id(n)} as TestFoo");

            Assert.That(reader.Read(), Is.EqualTo(true));
            var foo = reader.Get<TestFoo>(0);
            Assert.That(foo.Name, Is.EqualTo("Andres"));
            Assert.That(foo.Title, Is.EqualTo("Developer"));
            Assert.That(foo.Id, Is.GreaterThan(-1));
        }

        [Test]
        public async void CreateUsingExecute()
        {
            var reference = Guid.NewGuid();
            await this.neoClient.ExecuteAsync("CREATE (n:Person  { name : 'Andres', title : 'Developer', reference : '" + reference + "' })");

            var reader = await this.neoClient.QueryAsync("MATCH n WHERE n.reference = '" + reference + "' RETURN n");
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