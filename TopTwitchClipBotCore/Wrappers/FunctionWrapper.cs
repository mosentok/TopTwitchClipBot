using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using TopTwitchClipBotCore.Exceptions;
using TopTwitchClipBotCore.Extensions;
using TopTwitchClipBotCore.Models;
using TopTwitchClipBotModel;

namespace TopTwitchClipBotCore.Wrappers
{
    public class FunctionWrapper : IFunctionWrapper, IDisposable
    {
        readonly HttpClient _HttpClient = new HttpClient();
        readonly string _ChannelConfigEndpointFormat;
        readonly string _BroadcasterConfigEndpointFormat;
        readonly string _FunctionsKeyHeaderName;
        readonly string _GetChannelConfigFunctionKey;
        readonly string _PostChannelConfigFunctionKey;
        readonly string _PostBroadcasterConfigFunctionKey;
        readonly string _DeleteBroadcasterConfigFunctionKey;
        public FunctionWrapper(IConfigurationWrapper configWrapper)
        {
            _ChannelConfigEndpointFormat = configWrapper["ChannelConfigEndpointFormat"];
            _BroadcasterConfigEndpointFormat = configWrapper["BroadcasterConfigEndpointFormat"];
            _FunctionsKeyHeaderName = configWrapper["FunctionsKeyHeaderName"];
            _GetChannelConfigFunctionKey = configWrapper["GetChannelConfigFunctionKey"];
            _PostChannelConfigFunctionKey = configWrapper["PostChannelConfigFunctionKey"];
            _PostBroadcasterConfigFunctionKey = configWrapper["PostBroadcasterConfigFunctionKey"];
            _DeleteBroadcasterConfigFunctionKey = configWrapper["DeleteBroadcasterConfigFunctionKey"];
        }
        public async Task<ChannelConfigContainer> GetChannelConfigAsync(decimal channelId)
        {
            var requestUri = string.Format(_ChannelConfigEndpointFormat, channelId);
            var response = await _HttpClient.GetWithHeaderAsync(requestUri, _FunctionsKeyHeaderName, _GetChannelConfigFunctionKey);
            if (!response.IsSuccessStatusCode)
                throw new FunctionHelperException($"Error getting channel config for channel '{channelId}'. Status code '{response.StatusCode.ToString()}'. Reason phrase '{response.ReasonPhrase}'.");
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ChannelConfigContainer>(content);
        }
        public async Task<ChannelConfigContainer> PostChannelConfigAsync(decimal channelId, ChannelConfigContainer container)
        {
            var requestUri = string.Format(_ChannelConfigEndpointFormat, channelId);
            var response = await _HttpClient.PostObjectWithHeaderAsync(requestUri, container, _FunctionsKeyHeaderName, _PostChannelConfigFunctionKey);
            if (!response.IsSuccessStatusCode)
                throw new FunctionHelperException($"Error posting channel config for channel '{channelId}'. Status code '{response.StatusCode.ToString()}'. Reason phrase '{response.ReasonPhrase}'.");
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ChannelConfigContainer>(content);
        }
        public async Task<PostBroadcasterConfigResponse> PostBroadcasterConfigAsync(decimal channelId, string broadcaster, BroadcasterConfigContainer container)
        {
            var requestUri = string.Format(_BroadcasterConfigEndpointFormat, channelId, broadcaster);
            var response = await _HttpClient.PostObjectWithHeaderAsync(requestUri, container, _FunctionsKeyHeaderName, _PostBroadcasterConfigFunctionKey);
            if (!response.IsSuccessStatusCode)
                throw new FunctionHelperException($"Error posting channel top clip config for channel '{channelId}'. Status code '{response.StatusCode.ToString()}'. Reason phrase '{response.ReasonPhrase}'.");
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<PostBroadcasterConfigResponse>(content);
        }
        public async Task<ChannelConfigContainer> DeleteChannelTopClipConfigAsync(decimal channelId)
        {
            var requestUri = string.Format(_BroadcasterConfigEndpointFormat, channelId, string.Empty);
            var response = await _HttpClient.DeleteWithHeaderAsync(requestUri, _FunctionsKeyHeaderName, _DeleteBroadcasterConfigFunctionKey);
            if (!response.IsSuccessStatusCode)
                throw new FunctionHelperException($"Error deleting all channel top clip configs for channel '{channelId}'. Status code '{response.StatusCode.ToString()}'. Reason phrase '{response.ReasonPhrase}'.");
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ChannelConfigContainer>(content);
        }
        public async Task<ChannelConfigContainer> DeleteChannelTopClipConfigAsync(decimal channelId, string broadcaster)
        {
            var requestUri = string.Format(_BroadcasterConfigEndpointFormat, channelId, broadcaster);
            var response = await _HttpClient.DeleteWithHeaderAsync(requestUri, _FunctionsKeyHeaderName, _DeleteBroadcasterConfigFunctionKey);
            if (!response.IsSuccessStatusCode)
                throw new FunctionHelperException($"Error deleting channel top clip config for channel '{channelId}'. Status code '{response.StatusCode.ToString()}'. Reason phrase '{response.ReasonPhrase}'.");
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ChannelConfigContainer>(content);
        }
        public void Dispose()
        {
            _HttpClient?.Dispose();
        }
    }
}
