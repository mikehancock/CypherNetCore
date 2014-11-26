namespace CypherNet.Core
{
    internal class NeoRootResponse
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initialises a new instance of the <see cref="NeoRootResponse"/> class.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <param name="management">
        /// The management.
        /// </param>
        public NeoRootResponse(string data, string management)
        {
            this.Data = data;
            this.Management = management;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the data.
        /// </summary>
        public string Data { get; private set; }

        /// <summary>
        /// Gets the management.
        /// </summary>
        public string Management { get; private set; }

        #endregion
    }

    internal class NeoDataRootResponse
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initialises a new instance of the <see cref="NeoDataRootResponse"/> class.
        /// </summary>
        /// <param name="transaction">
        /// The transaction.
        /// </param>
        /// <param name="neo4jVersion">
        /// The neo 4 j version.
        /// </param>
        public NeoDataRootResponse(string transaction, string neo4jVersion)
        {
            this.Transaction = transaction;
            this.Neo4JVersion = neo4jVersion;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the neo 4 j version.
        /// </summary>
        public string Neo4JVersion { get; private set; }

        /// <summary>
        /// Gets the transaction.
        /// </summary>
        public string Transaction { get; private set; }

        #endregion
    }
}