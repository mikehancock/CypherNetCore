namespace CypherTwo.Core
{
    using System.Threading.Tasks;

    internal interface ISendRestCommandsToNeo
    {
        Task<NeoResponse> SendCommandAsync(string command);

        Task LoadServiceRootAsync();
    }
}