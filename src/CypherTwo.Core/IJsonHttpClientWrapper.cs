namespace CypherTwo.Core
{
    using System.Threading.Tasks;

    public interface IJsonHttpClientWrapper
    {
        Task<string> PostAsync(string url, string request);

        Task<string> DeleteAsync(string url);

        Task<string> GetAsync(string url);
    }
}