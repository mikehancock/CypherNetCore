namespace CypherNet.Core.Tests
{
    using System;
    using System.Text;
    using System.Transactions;

    using CypherNet.Core;

    using NUnit.Framework;

    [TestFixture(Category = "Integration")]
    public class TransactionalIntegrationTests
    {
        private static Random random = new Random((int)DateTime.Now.Ticks);

        private GraphStore graphStore;

        [Test]
        public async void TransactionCommitsOnScopeComplete([Values(false, true)] bool performCommit)
        {
            this.graphStore = new GraphStore("http://localhost:7474/", "neo4j", "longbow");
            await this.graphStore.InitializeAsync();
            var neoClient = this.graphStore.GetClient();
            var randomText = this.RandomString(12);
            int newId = -1;
            using (var ts = new TransactionScope())
            {
                var reader = await neoClient.QueryAsync("CREATE (n:Person  { name:'" + randomText + "' }) RETURN Id(n)");
                Assert.That(reader.Read(), Is.EqualTo(true));
                newId = reader.Get<int>(0);
                Assert.That(newId, Is.GreaterThan(-1));
                if (performCommit)
                {
                    ts.Complete();
                }
            }

            var queryReader = await neoClient.QueryAsync("MATCH (n:Person) WHERE n.name ='" + randomText + "' RETURN Id(n)");
            Assert.That(queryReader.Read(), Is.EqualTo(performCommit));
            if (performCommit)
            {
                Assert.That(queryReader.Get<int>(0), Is.EqualTo(newId));
            }
        }

        private string RandomString(int size)
        {
            var builder = new StringBuilder();

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