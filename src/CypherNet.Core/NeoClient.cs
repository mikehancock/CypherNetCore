namespace CypherNet.Core
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

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

            return new NeoClient(this.dataRoot);
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

    /// <summary>
    /// The neo client.
    /// </summary>
    public class NeoClient : INeoClient
    {
        #region Fields

        private readonly ApiClientFactory restCommandFactory;

        #endregion

        #region Constructors and Destructors

        internal NeoClient(NeoDataRootResponse baseUrl)
        {
            this.restCommandFactory = new ApiClientFactory(baseUrl, new JsonHttpClientWrapper());
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The execute async.
        /// </summary>
        /// <param name="cypher">
        /// The cypher.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task ExecuteAsync(string cypher)
        {
            await this.ExecuteCore(cypher);
        }

        /// <summary>
        /// The query async.
        /// </summary>
        /// <param name="cypher">
        /// The cypher.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<ICypherDataReader> QueryAsync(string cypher)
        {
            var neoResponse = await this.ExecuteCore(cypher);

            return new CypherDataReader(neoResponse);
        }

        #endregion

        #region Methods

        private async Task<NeoResponse> ExecuteCore(string cypher)
        {
            var neoResponse = await this.restCommandFactory.GetApiClient().SendCommandAsync(cypher);
            if (neoResponse.errors != null && neoResponse.errors.Any())
            {
                throw new Exception(string.Join(Environment.NewLine, neoResponse.errors.Select(error => error.ToObject<string>())));
            }

            return neoResponse;
        }

        #endregion
    }
}