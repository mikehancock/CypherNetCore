namespace CypherTwo.Core
{
    using System.Threading.Tasks;

    public interface INeoClient
    {
        Task InitialiseAsync();

        Task<ICypherDataReader> QueryAsync(string cypher);

        Task ExecuteAsync(string cypher);
    }
}