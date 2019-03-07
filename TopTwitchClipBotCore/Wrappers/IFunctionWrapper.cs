using System.Threading.Tasks;
using TopTwitchClipBotModel;

namespace TopTwitchClipBotCore.Wrappers
{
    public interface IFunctionWrapper
    {
        Task<ChannelConfigContainer> GetChannelConfigAsync(decimal channelId);
        Task<ChannelConfigContainer> PostChannelConfigAsync(decimal channelId, ChannelConfigContainer container);
        Task<ChannelConfigContainer> PostBroadcasterConfigAsync(decimal channelId, string broadcaster, BroadcasterConfigContainer container);
        Task DeleteChannelTopClipConfigAsync(decimal channelId);
        Task DeleteChannelTopClipConfigAsync(decimal channelId, string broadcaster);
    }
}