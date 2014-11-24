// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISendRestCommandsToNeo.cs" Copyright (c) 2013 Plaza De Armas Ltd>
//   Copyright (c) 2013 Plaza De Armas Ltd
// </copyright>
// <summary>
//   Defines the ISendRestCommandsToNeo type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CypherTwo.Core
{
    using System.Threading.Tasks;

    internal interface ISendRestCommandsToNeo
    {
        #region Public Methods and Operators

        /// <summary>
        /// The send command async.
        /// </summary>
        /// <param name="command">
        /// The command.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task<NeoResponse> SendCommandAsync(string command);

        #endregion
    }
}