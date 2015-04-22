namespace CypherNet.Core
{
    using System;
    using System.Linq;

    using Newtonsoft.Json;

    /// <summary>
    /// The graph store.
    /// </summary>
    public class GraphStore
    {
        #region Fields

        private static readonly int[] MinimumVersionNumber = { 2, 0, 0 };

        private string baseUrl;

        private NeoDataRootResponse dataRoot;

        private IJsonHttpClientWrapper httpClient;

        private NeoRootResponse serviceRoot;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initialises a new instance of the <see cref="GraphStore"/> class, usable before Neo4J version 2.2.0.
        /// </summary>
        /// <param name="baseUrl">
        /// The base url.
        /// </param>
        public GraphStore(string baseUrl)
            : this(baseUrl, new JsonHttpClientWrapper())
        {
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="GraphStore"/> class.
        /// </summary>
        /// <param name="baseUrl">
        /// The base url.
        /// </param>
        /// <param name="userName">User name for the data connection</param>
        /// <param name="password">Password for the data connection</param>
        public GraphStore(string baseUrl, string userName, string password)
            : this(baseUrl, new JsonHttpClientWrapper(userName, password))
        {
        }

        internal GraphStore(string baseUrl, IJsonHttpClientWrapper httpClient)
        {
            this.baseUrl = baseUrl;
            this.httpClient = httpClient;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The get session.
        /// </summary>
        /// <returns>
        /// The <see cref="INeoClient"/>.
        /// </returns>
        public INeoClient GetClient()
        {
            if (this.dataRoot == null)
                throw new InvalidOperationException("Initialize must be called before using GetClient()");

            return new NeoClient(new ApiClientFactory(this.dataRoot, new JsonHttpClientWrapper("neo4j", "longbow")));
        }

        /// <summary>
        /// The initialize.
        /// </summary>
        public void Initialize()
        {
            var result = this.httpClient.GetAsync(this.baseUrl).Result;
            this.serviceRoot = JsonConvert.DeserializeObject<NeoRootResponse>(result);
            var dataRootResult = this.httpClient.GetAsync(this.serviceRoot.Data).Result;
            this.dataRoot = JsonConvert.DeserializeObject<NeoDataRootResponse>(dataRootResult);
            this.AssertVersion(this.dataRoot);
        }

        #endregion

        private void AssertVersion(NeoDataRootResponse response)
        {
            var serverversion = response.Neo4JVersion;
            if (string.IsNullOrEmpty(serverversion))
            {
                throw new Exception("Cannot read Neo4j Server Version");
            }

            var versionNumberStrings = serverversion.Split(new[] { '.', '-' }).Take(3).ToArray();
            for (var i = 0; i < versionNumberStrings.Count(); i++)
            {
                var versionNumberString = versionNumberStrings[i];
                var versionNumber = 0;
                if (!int.TryParse(versionNumberString, out versionNumber))
                {
                    throw new Exception("Invalid Neo4j Server Version: " + serverversion);
                }

                if (versionNumber < MinimumVersionNumber[i])
                {
                    throw new Exception(string.Format("Incompatible Neo4j Server Version: {0}. Cypher.Net is currently only compatible with Neo4j versions {1} and above", serverversion, string.Join(".", MinimumVersionNumber)));
                }
            }
        }
    }
}