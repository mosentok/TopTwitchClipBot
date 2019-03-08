using System.Threading.Tasks;
using TopTwitchClipBotCore.Models;
using TopTwitchClipBotModel;

namespace TopTwitchClipBotCore.Wrappers
{
    public interface IFunctionWrapper
    {
        Task<ChannelConfigContainer> GetChannelConfigAsync(decimal channelId);
        Task<ChannelConfigContainer> PostChannelConfigAsync(decimal channelId, ChannelConfigContainer container);
        Task<PostBroadcasterConfigResponse> PostBroadcasterConfigAsync(decimal channelId, string broadcaster, BroadcasterConfigContainer container);
        Task<ChannelConfigContainer> DeleteChannelTopClipConfigAsync(decimal channelId);
        Task<ChannelConfigContainer> DeleteChannelTopClipConfigAsync(decimal channelId, string broadcaster);
    }
}