namespace CypherTwo.Core
{
    using System;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;

    public class JsonHttpClientWrapper : IJsonHttpClientWrapper
    {
        public async Task<string> PostAsync(string url, string request)
        {
            using (var httpClient = new HttpClient())
            {
                var httpContent = request == null ? null : new StringContent(request, Encoding.Unicode, "application/json");
                var response = await httpClient.PostAsync(url, httpContent);

                if (!response.IsSuccessStatusCode)
                    throw new Exception(response.Content.ReadAsStringAsync().Result);

                return await response.Content.ReadAsStringAsync();
            }
        }

        public async Task<string> DeleteAsync(string url)
        {
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.DeleteAsync(url);

                if (!response.IsSuccessStatusCode)
                    throw new Exception(response.Content.ReadAsStringAsync().Result);

                return await response.Content.ReadAsStringAsync();
            }
        }

        public async Task<string> GetAsync(string url)
        {
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                    throw new Exception(response.Content.ReadAsStringAsync().Result);

                return await response.Content.ReadAsStringAsync();
            }
        }
    }
}
