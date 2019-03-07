using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using TopTwitchClipBotFunctions.Exceptions;
using TopTwitchClipBotFunctions.Extensions;
using TopTwitchClipBotFunctions.Models;

namespace TopTwitchClipBotFunctions.Wrappers
{
    public class TwitchWrapper : ITwitchWrapper, IDisposable
    {
        readonly HttpClient _HttpClient = new HttpClient();
        public async Task<GetUsersResponse> GetUsers(string endpoint, string clientId, string accept, string broadcaster)
        {
            var userEndpoint = $"{endpoint}?login={broadcaster}";
            var response = await _HttpClient.GetWithHeadersAsync(userEndpoint, new Header("Client-ID", clientId), new Header("Accept", accept));
            if (!response.IsSuccessStatusCode)
                throw new TwitchException();
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<GetUsersResponse>(content);
        }
        public async Task<GetClipsResponse> GetClips(string endpoint, string clientId, string accept)
        {
            var response = await _HttpClient.GetWithHeadersAsync(endpoint, new Header("Client-ID", clientId), new Header("Accept", accept));
            if (!response.IsSuccessStatusCode)
                throw new TwitchException();
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<GetClipsResponse>(content);
        }
        public void Dispose()
        {
            _HttpClient?.Dispose();
        }
    }
}
