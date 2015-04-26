namespace CypherNet.Core.Tests
{
    using System;

    using CypherNet.Core;

    using NUnit.Framework;

    [TestFixture(Category = "Integration")]
    public class IntegrationTests
    {
        private GraphStore graphStore;
        private INeoClient neoClient;
        private IJsonHttpClientWrapper httpClientWrapper;

        [TestFixtureSetUp]
        public void SetupOnce()
        {
            this.graphStore = new GraphStore("http://localhost:7474/", new JsonHttpClientWrapper("neo4j", "longbow"));
            this.graphStore.InitializeAsync().Wait();
        }

        [SetUp]
        public void SetupBeforeEachTest()
        {
            this.httpClientWrapper = new JsonHttpClientWrapper("neo4j", "longbow");
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

        [Test]
        public void InvalidCypherThrowsException()
        {
            var ex = Assert.Throws<AggregateException>(() => this.neoClient.ExecuteAsync("This is not a valid Cypher Statement.").Wait());
            Assert.That(ex.InnerException.Message, Is.StringStarting("Neo.ClientError.Statement.InvalidSyntax: "));
        }

        [Test, Ignore]
        public async void CreateAndReturnNodeDataUsingQuery()
        {
            var reader = await this.neoClient.QueryAsync("CREATE (NewNode:brewery {name:\"Plzensky Prazdroj\"}) RETURN NewNode as NewNode, id(NewNode) as NewNode__Id, labels(NewNode) as NewNode__Labels;");

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