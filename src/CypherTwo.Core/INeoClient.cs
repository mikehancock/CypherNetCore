namespace CypherTwo.Core
{
    using System.Threading.Tasks;

    public interface INeoClient
    {

        Task<ICypherDataReader> QueryAsync(string cypher);

        Task ExecuteAsync(string cypher);
    }
}