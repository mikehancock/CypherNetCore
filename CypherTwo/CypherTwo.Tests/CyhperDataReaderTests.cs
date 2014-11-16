namespace CypherTwo.Tests
{
    using System;
    using System.Linq;

    using CypherTwo.Core;

    using Newtonsoft.Json;

    using NUnit.Framework;

    [TestFixture]
    public class CyhperDataReaderTests
    {
        #region Constants
        private const string Response = @"{
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

        private ICypherDataReader dataReader;

        [SetUp]
        public void SetupBeforeEachTest()
        {
            var neoResponse = JsonConvert.DeserializeObject<NeoResponse>(Response);
            this.dataReader = new CypherDataReader(neoResponse);
        }

        [Test]
        public void DataReaderReturnsColumns()
        {
            Assert.That(this.dataReader.Columns.Count(), Is.EqualTo(9));
        }

        [Test]
        public void DataReaderReturnsColumnNames()
        {
            Assert.That(this.dataReader.Columns[6], Is.EqualTo("Movie"));
        }

        [Test]
        public void NextReturnsTrueWhenNotAtEnd()
        {
            Assert.That(this.dataReader.Read(), Is.EqualTo(true));
        }

        [Test]
        public void NextReturnsFalseWhenAtEnd()
        {
            this.dataReader.Read();
            this.dataReader.Read();
            Assert.That(this.dataReader.Read(), Is.EqualTo(false));
        }

        [Test]
        public void GetTHrowsExceptionForNegativeIndex()
        {
            Assert.Throws<IndexOutOfRangeException>(() => this.dataReader.Get<int>(-1));
        }

        [Test]
        public void GetTHrowsExceptionForTooLargeIndex()
        {
            Assert.Throws<IndexOutOfRangeException>(() => this.dataReader.Get<int>(10));
        }

        [Test]
        public void GetThrowsExceptionWhenRowCountExceeded()
        {
            this.dataReader.Read();
            this.dataReader.Read();
            this.dataReader.Read();
            Assert.Throws<InvalidOperationException>(() => this.dataReader.Get<int>(1));
        }

        [Test]
        public void GetReturnsIntFromFirstRow()
        {
            this.dataReader.Read();
            var actual = this.dataReader.Get<int>(1);
            Assert.That(actual, Is.EqualTo(3745));
        }

        [Test]
        public void GetReturnsObjectFromFirstRow()
        {
            this.dataReader.Read();
            var actual = this.dataReader.Get<TestFoo>(0);
            Assert.That(actual.Age, Is.EqualTo(33));
            Assert.That(actual.Name, Is.EqualTo("mark"));
        }
    }
}
