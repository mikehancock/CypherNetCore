namespace CypherNet.Core.Tests
{
    using System;

    using CypherNet.Core;

    using Newtonsoft.Json;

    using NUnit.Framework;

    [TestFixture(Category = "Integration")]
    public class GraphStoreTests
    {
        [Test]
        public void InitialiseThrowsExecptionWithInvalidUrl()
        {
            var graphStore = new GraphStore("http://www.google.com/");
            Assert.Throws<JsonReaderException>(graphStore.Initialize);
        }

        [Test]
        public void CallingGetClientBeforeInitializeThrowsInvalidOperationException()
        {
            var graphStore = new GraphStore("http://localhost:7474/");
            Assert.Throws<InvalidOperationException>(() => graphStore.GetClient());
        }

        [Test]
        public void InitializeThenGetClientReturnsClient()
        {
            var graphStore = new GraphStore("http://localhost:7474/", "neo4j", "longbow");
            graphStore.Initialize();
            var client = graphStore.GetClient();

            Assert.That(client, Is.InstanceOf<NeoClient>());
        }
    }
}
