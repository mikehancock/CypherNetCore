namespace CypherNet.Core.Tests
{
    using System;

    using CypherNet.Core;

    using FakeItEasy;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    using NUnit.Framework;

    [TestFixture]
    public class NeoClientTests
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

        private INeoClient neoClient;

        private ISendRestCommandsToNeo neoApi;

        private IApiClientFactory apiClientFactory;

        [SetUp]
        public void SetupBeforeEachTest()
        {
            this.neoApi = A.Fake<ISendRestCommandsToNeo>();
            A.CallTo(() => this.neoApi.SendCommandAsync(A<string>._)).Returns(JsonConvert.DeserializeObject<NeoResponse>(Response));
            this.apiClientFactory = A.Fake<IApiClientFactory>();
            A.CallTo(() => this.apiClientFactory.GetApiClient()).Returns(this.neoApi);
            this.neoClient = new NeoClient(this.apiClientFactory);
        }

        [Test]
        public void NeoClientReturnsDataReader()
        {
            var dataReader = this.neoClient.QueryAsync("whatever").Result;
            Assert.That(dataReader, Is.InstanceOf<ICypherDataReader>());
        }

        [Test]
        public void QueryCallsSendCommandAsync()
        {
            var dataReader = this.neoClient.QueryAsync("whatever").Result;
            A.CallTo(() => this.neoApi.SendCommandAsync("whatever")).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void ExecuteCallsSendCommandAsync()
        {
            this.neoClient.ExecuteAsync("whatever").Wait();
            A.CallTo(() => this.neoApi.SendCommandAsync("whatever")).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void NeoErrorsArePropagated()
        {
            var json = @"{
  ""results"" : [ ],
  ""errors"" : [ {
    ""code"" : ""Neo.ClientError.Statement.InvalidSyntax"",
    ""message"" : ""Invalid input 'T': expected <init> (line 1, column 1)\n\""This is not a valid Cypher Statement.\""\n ^""
  } ]
}";
            var response = JsonConvert.DeserializeObject<NeoResponse>(json);
            A.CallTo(() => this.neoApi.SendCommandAsync("whatever")).Returns(response);
            
            var exception = Assert.Throws<AggregateException>(() => this.neoClient.ExecuteAsync("whatever").Wait());

            Assert.That(exception.InnerException, Is.InstanceOf<Exception>());
            Assert.That(exception.InnerException.Message, Is.StringStarting("Neo.ClientError.Statement.InvalidSyntax: "));
        }
    }
}
