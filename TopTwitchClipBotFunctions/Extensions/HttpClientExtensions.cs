using System.Net.Http;
using System.Threading.Tasks;
using TopTwitchClipBotFunctions.Models;

namespace TopTwitchClipBotFunctions.Extensions
{
    public static class HttpClientExtensions
    {
        public static Task<HttpResponseMessage> GetWithHeadersAsync(this HttpClient httpClient, string requestUri, params Header[] headers)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, requestUri))
            {
                foreach (var header in headers)
                    request.Headers.Add(header.Name, header.Value);
                return httpClient.SendAsync(request);
            }
        }
    }
}
