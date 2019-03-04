using System.Threading.Tasks;
using TopTwitchClipBotModel;

namespace TopTwitchClipBotCore.Wrappers
{
    public interface IFunctionWrapper
    {
        Task<ChannelConfigContainer> GetChannelConfigAsync(decimal channelId);
        Task<ChannelConfigContainer> PostChannelConfigAsync(decimal channelId, ChannelConfigContainer container);
        Task<ChannelTopClipConfigContainer> PostChannelTopClipConfigAsync(decimal channelId, string broadcaster, ChannelTopClipConfigContainer container);
        Task DeleteChannelTopClipConfigAsync(decimal channelId);
        Task DeleteChannelTopClipConfigAsync(decimal channelId, string broadcaster);
    }
}