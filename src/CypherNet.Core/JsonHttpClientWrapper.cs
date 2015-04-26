namespace CypherNet.Core
{
    using System;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// The json http client wrapper.
    /// </summary>
    public class JsonHttpClientWrapper : IJsonHttpClientWrapper
    {
        private readonly bool useAuthentication;
        private readonly string base64AuthenticationString;

        /// <summary>
        /// Creates a http client wrapper for use prior to Neo4J 2.2.0, without credentials
        /// </summary>
        public JsonHttpClientWrapper()
        {
            this.useAuthentication = false;
            this.base64AuthenticationString = null;
        }

        /// <summary>
        /// Creates a http client wrapper for use with Neo4J 2.2.0+, with credentials.
        /// <param name="userName">The basic authentication user name</param>
        /// <param name="password">The basic authentication password</param>
        /// </summary>
        public JsonHttpClientWrapper(string userName, string password)
        {
            this.useAuthentication = true;
            var bytes = Encoding.UTF8.GetBytes(string.Format("{0}:{1}", userName, password));
            this.base64AuthenticationString = Convert.ToBase64String(bytes);
        }

        #region Public Methods and Operators

        /// <summary>
        /// The delete async.
        /// </summary>
        /// <param name="url">
        /// The url.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        /// <exception cref="Exception">
        /// </exception>
        public async Task<string> DeleteAsync(string url)
        {
            using (var httpClient = this.CreateHttpClient())
            {
                var response = await httpClient.DeleteAsync(url);
                return await GetResponseContent(response);
            }
        }

        /// <summary>
        /// The get async.
        /// </summary>
        /// <param name="url">
        /// The url.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        /// <exception cref="Exception">
        /// </exception>
        public async Task<string> GetAsync(string url)
        {
            using (var httpClient = this.CreateHttpClient())
            {
                var response = await httpClient.GetAsync(url);
                return await GetResponseContent(response);
            }
        }

        /// <summary>
        /// The post async.
        /// </summary>
        /// <param name="url">
        /// The url.
        /// </param>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        /// <exception cref="Exception">
        /// </exception>
        public async Task<string> PostAsync(string url, string request)
        {
            using (var httpClient = this.CreateHttpClient())
            {
                var httpContent = request == null ? null : new StringContent(request, Encoding.Unicode, "application/json");
                var response = httpClient.PostAsync(url, httpContent)
                    .ConfigureAwait(continueOnCapturedContext: false).GetAwaiter().GetResult();
                return await GetResponseContent(response);
            }
        }

        private static async Task<string> GetResponseContent(HttpResponseMessage response)
        {
            string content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(content);
            }

            return content;
        }

        #endregion

        private HttpClient CreateHttpClient()
        {
            HttpMessageHandler handler = new HttpClientHandler();
            var client = new HttpClient();
            if (this.useAuthentication)
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", this.base64AuthenticationString);
            }

            return client;
        }
    }
}
