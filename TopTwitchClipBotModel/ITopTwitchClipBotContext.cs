using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace TopTwitchClipBotModel
{
    public interface ITopTwitchClipBotContext
    {
        DbSet<ChannelConfig> ChannelConfigs { get; set; }
        DbSet<BroadcasterConfig> BroadcasterConfigs { get; set; }
        DbSet<BroadcasterHistory> BroadcasterHistories { get; set; }

        Task DeleteChannelTopClipConfigAsync(decimal channelId);
        Task DeleteChannelTopClipConfigAsync(decimal channelId, string broadcaster);
        Task<ChannelConfigContainer> GetChannelConfigAsync(decimal channelId);
        Task<List<PendingBroadcasterConfig>> GetBroadcasterConfigsAsync();
        Task<List<BroadcasterHistoryContainer>> InsertBroadcasterHistoriesAsync(List<BroadcasterHistoryContainer> containers);
        Task<ChannelConfigContainer> SetChannelConfigAsync(decimal channelId, ChannelConfigContainer container);
        Task<BroadcasterConfigContainer> SetBroadcasterConfigAsync(decimal channelId, string broadcaster, BroadcasterConfigContainer container);
    }
}