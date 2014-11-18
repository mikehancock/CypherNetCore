namespace CypherTwo.Tests
{
    using System;
    using System.Net.Http;
    using System.Text;
    using System.Transactions;
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
            this.neoApi = new NonTransactionalNeoRestApiClient(this.httpClientWrapper, "http://localhost:7474/db/data");
            this.neoClient = new NeoClient(this.neoApi);
            this.neoClient.InitialiseAsync().Wait();
        }

        [Test]
        public void InitialiseThrowsExecptionWithInvalidUrl()
        {
            this.neoApi = new NonTransactionalNeoRestApiClient(this.httpClientWrapper, "http://localhost:1111/");
            this.neoClient = new NeoClient(this.neoApi);
            Assert.Throws<InvalidOperationException>(() =>
                {
                    try
                    {
                        this.neoClient.InitialiseAsync().Wait();
                    }
                    catch (AggregateException ex)
                    {
                        throw ex.InnerException;
                    }
                });
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

    [TestFixture]
    internal class TransactionTests
    {
        private static Random random = new Random((int)DateTime.Now.Ticks);

        private INeoClient neoClient;

        private ISendRestCommandsToNeo neoApi;

        private IJsonHttpClientWrapper httpClientWrapper;

        [Test]
        public async void TransactionCommitsOnScopeComplete()
        {
            this.neoClient = new NeoClient("http://localhost:7474/db/data");
            await this.neoClient.InitialiseAsync();
            var randomText = this.RandomString(12);
            int newId = -1;
            using (var ts = new TransactionScope())
            {
                var reader = await this.neoClient.QueryAsync("CREATE (n:Person  { name:'" + randomText + "' }) RETURN Id(n)");
                Assert.That(reader.Read(), Is.EqualTo(true));
                newId = reader.Get<int>(0);
                Assert.That(newId, Is.GreaterThan(-1));

                ts.Complete();
            }

            var queryReader = await this.neoClient.QueryAsync("MATCH (n:Person) WHERE name ='" + randomText + "' RETURN Id(n)");
            Assert.That(queryReader.Read(), Is.EqualTo(true));
            Assert.That(queryReader.Get<int>(0), Is.EqualTo(newId));
        }

        private string RandomString(int size)
        {
            StringBuilder builder = new StringBuilder();

            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor((26 * random.NextDouble()) + 65)));
                builder.Append(ch);
            }

            return builder.ToString();
        }
    }
}