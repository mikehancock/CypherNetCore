namespace CypherTwo.Tests
{
    using CypherTwo.Core;

    using FakeItEasy;

    using NUnit.Framework;

    [TestFixture]
    public class NeoClientTests
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

        private INeoClient neoClient;

        private ISendRestCommandsToNeo neoApi;

        [SetUp]
        public void SetupBeforeEachTest()
        {
            this.neoApi = A.Fake<ISendRestCommandsToNeo>();
            A.CallTo(() => this.neoApi.SendCommandAsync(A<string>._)).Returns(response);
            this.neoClient = new NeoClient(this.neoApi);
        }

        [Test]
        public void NeoClientReturnsDataReader()
        {
            var dataReader = this.neoClient.QueryAsync("whatever").Result;
            Assert.That(dataReader, Is.InstanceOf<ICypherDataReader>());
        }
    }
}
