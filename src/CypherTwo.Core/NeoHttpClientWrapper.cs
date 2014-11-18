namespace CypherTwo.Core
{
    using System;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;

    public interface IJsonHttpClientWrapper
    {
        Task<string> PostAsync(string url, string request);

        Task<string> GetAsync(string url);
    }

    public class JsonHttpClientWrapper : IJsonHttpClientWrapper
    {
        private readonly HttpClient httpClient;

        public JsonHttpClientWrapper(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<string> PostAsync(string url, string request)
        {
            var httpContent = request == null ? null : new StringContent(request, Encoding.Unicode, "application/json");
            var response = await this.httpClient.PostAsync(url, httpContent);

            if (!response.IsSuccessStatusCode)
                throw new Exception(response.Content.ReadAsStringAsync().Result);

            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> GetAsync(string url)
        {
            var response = await this.httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                throw new Exception(response.Content.ReadAsStringAsync().Result);

            return await response.Content.ReadAsStringAsync();
        }
    }
}
