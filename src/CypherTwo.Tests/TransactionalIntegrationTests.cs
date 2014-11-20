namespace CypherTwo.Tests
{
    using System;
    using System.Text;
    using System.Transactions;
    using CypherTwo.Core;
    using NUnit.Framework;

    [TestFixture]
    public class TransactionalIntegrationTests
    {   
        private static Random random = new Random((int)DateTime.Now.Ticks);

        private INeoClient neoClient;

        [Test]
        public void TransactionCommitsOnScopeComplete()
        {
            this.neoClient = new NeoClient("http://localhost:7474/db/data");
            this.neoClient.InitialiseAsync().Wait();
            var randomText = this.RandomString(12);
            int newId = -1;
            using (var ts = new TransactionScope())
            {
                var reader = this.neoClient.QueryAsync("CREATE (n:Person  { name:'" + randomText + "' }) RETURN Id(n)").Result;
                Assert.That(reader.Read(), Is.EqualTo(true));
                newId = reader.Get<int>(0);
                Assert.That(newId, Is.GreaterThan(-1));

                ts.Complete();
            }

            var queryReader = this.neoClient.QueryAsync("MATCH (n:Person) WHERE n.name ='" + randomText + "' RETURN Id(n)").Result;
            Assert.That(queryReader.Read(), Is.EqualTo(true));
            Assert.That(queryReader.Get<int>(0), Is.EqualTo(newId));
        }

        [Test]
        public void TransactionRollsbackWhenNotScopeComplete()
        {
            this.neoClient = new NeoClient("http://localhost:7474/db/data");
            this.neoClient.InitialiseAsync().Wait();
            var randomText = this.RandomString(12);
            int newId = -1;
            using (var ts = new TransactionScope())
            {
                var reader = this.neoClient.QueryAsync("CREATE (n:Person  { name:'" + randomText + "' }) RETURN Id(n)").Result;
                Assert.That(reader.Read(), Is.EqualTo(true));
                newId = reader.Get<int>(0);
                Assert.That(newId, Is.GreaterThan(-1));
            }

            var queryReader = this.neoClient.QueryAsync("MATCH (n:Person) WHERE n.name ='" + randomText + "' RETURN Id(n)").Result;
            Assert.That(queryReader.Read(), Is.EqualTo(false));
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