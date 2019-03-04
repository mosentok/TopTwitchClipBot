using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using TopTwitchClipBotFunctions.Extensions;
using TopTwitchClipBotFunctions.Models;

namespace TopTwitchClipBotFunctions.Wrappers
{
    public class TwitchWrapper : ITwitchWrapper, IDisposable
    {
        readonly HttpClient _HttpClient = new HttpClient();
        public async Task<GetClipsResponse> GetClips(string endpoint, string clientId, string accept)
        {
            var response = await _HttpClient.GetWithHeadersAsync(endpoint, new Header("Client-ID", clientId), new Header("Accept", accept));
            if (!response.IsSuccessStatusCode)
                throw new System.Exception("TODO");
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<GetClipsResponse>(content);
        }
        public void Dispose()
        {
            _HttpClient?.Dispose();
        }
    }
}
