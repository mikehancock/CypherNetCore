namespace CypherNet.Core
{
    using System;

    using Newtonsoft.Json;

    /// <summary>
    /// The graph store.
    /// </summary>
    public class GraphStore
    {
        #region Fields

        private string baseUrl;

        private NeoDataRootResponse dataRoot;

        private IJsonHttpClientWrapper httpClient;

        private NeoRootResponse serviceRoot;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initialises a new instance of the <see cref="GraphStore"/> class.
        /// </summary>
        /// <param name="baseUrl">
        /// The base url.
        /// </param>
        public GraphStore(string baseUrl)
            : this(baseUrl, new JsonHttpClientWrapper())
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

            return new NeoClient(new ApiClientFactory(this.dataRoot, new JsonHttpClientWrapper()));
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
        }

        #endregion
    }
}