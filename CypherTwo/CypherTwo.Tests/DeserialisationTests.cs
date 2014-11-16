namespace CypherTwo.Tests
{
    using System.Linq;

    using CypherTwo.Core;

    using FakeItEasy;

    using NUnit.Framework;

    [TestFixture]
    public class DeserialisationTests
    {
        #region Constants
        private const string response = @"{
   ""commit"":""http://localhost:7474/db/data/transaction/6/commit"",
   ""results"":[
      {
         ""columns"":[
            ""Actor"",
            ""Actor__Id"",
            ""Actor__Labels"",
            ""ActedIn"",
            ""ActedIn__Id"",
            ""ActedIn__Type"",
            ""Movie"",
            ""Movie__Id"",
            ""Movie__Labels""
         ],
         ""data"":[{ ""row"":
            [
               {
                  ""age"":33,
                  ""name"":""mark""
               },
               3745,
                [""person""],
               {
 
               },
               39490,
               ""IS_A"",
               {
                  ""title"":""developer""
               },
               3746,
               []
            ]},{""row"":[
               {
                  ""age"":21,
                  ""name"":""John""
               },
               3747,
               [""person""],
               {
 
               },
               39491,
               ""IS_A"",
               {
                  ""title"":""leg""
               },
               3748,
               []
            ]}
         ]
      }
   ],
   ""transaction"":{
      ""expires"":""Tue, 30 Jul 2013 15:57:59 +0000""
   },
   ""errors"":[
 
   ]
}";
        #endregion

        private Deserialiser deserialiser;

        private ISendRestCommandsToNeo neoApi;

        [SetUp]
        public void SetupBeforeEachTest()
        {
            this.deserialiser = new Deserialiser();
        }

        [Test]
        public void CanDeserialiseRows()
        {
            var actual = deserialiser.Deserialise(response);

            Assert.That(actual.results.First().columns.Length, Is.EqualTo(9));
            Assert.That(actual.results.First().columns[6], Is.EqualTo("Movie"));
        }

        [Test]
        public void CanExtractNodeObject()
        {
            var actual = this.deserialiser.Deserialise(response);

            dynamic node = actual.results.First().data[0].row[0];
            int age = node.age;
            string name = node.name;
            Assert.That(age, Is.EqualTo(33));
            Assert.That(name, Is.EqualTo("mark"));
        }

        [Test]
        public void CanCastToObject()
        {
            var actual = this.deserialiser.Deserialise(response);

            var foo = actual.results.First().data[0].row[0].ToObject<TestFoo>();

            Assert.That(foo.Age, Is.EqualTo(33));
            Assert.That(foo.Name, Is.EqualTo("mark"));
        }

        [Test]
        public void CanGetIdFromRow()
        {
            var actual = this.deserialiser.Deserialise(response);

            var foo = actual.results.First().data[0].row[1].ToObject<int>();
            Assert.That(foo, Is.EqualTo(3745));
        }
    }

    public class TestFoo
    {
        public int Age { get; set; }
        public string Name { get; set; }
    }
}


