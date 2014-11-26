namespace CypherNet.Core
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