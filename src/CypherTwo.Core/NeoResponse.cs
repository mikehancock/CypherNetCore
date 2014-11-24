// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NeoResponse.cs" Copyright (c) 2013 Plaza De Armas Ltd>
//   Copyright (c) 2013 Plaza De Armas Ltd
// </copyright>
// <summary>
//   Defines the Datum type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CypherTwo.Core
{
    using Newtonsoft.Json.Linq;

    internal class Datum
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the row.
        /// </summary>
        public JToken[] row { get; set; }

        #endregion
    }

    internal class Result
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the columns.
        /// </summary>
        public string[] columns { get; set; }

        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        public Datum[] data { get; set; }

        #endregion
    }

    internal class NeoResponse
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the commit.
        /// </summary>
        public string Commit { get; set; }

        /// <summary>
        /// Gets or sets the errors.
        /// </summary>
        public JObject[] errors { get; set; }

        /// <summary>
        /// Gets or sets the results.
        /// </summary>
        public Result[] results { get; set; }

        #endregion
    }

    internal class Person
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        #endregion
    }
}