namespace CypherNet.Core.Tests
{
    using CypherNet.Core;

    using Newtonsoft.Json;

    using NUnit.Framework;

    [TestFixture]
    public class GraphStoreTests
    {
        [Test]
        public void InitialiseThrowsExecptionWithInvalidUrl()
        {
            var graphStore = new GraphStore("http://www.google.com/");
            Assert.Throws<JsonReaderException>(graphStore.Initialize);
        }   
    }
}
