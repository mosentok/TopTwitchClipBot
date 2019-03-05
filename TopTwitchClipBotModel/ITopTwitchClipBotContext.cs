using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace TopTwitchClipBotModel
{
    public interface ITopTwitchClipBotContext
    {
        DbSet<ChannelConfig> ChannelConfigs { get; set; }
        DbSet<ChannelTopClipConfig> ChannelTopClipConfigs { get; set; }
        DbSet<TopClipHistory> TopClipHistories { get; set; }

        Task DeleteChannelTopClipConfigAsync(decimal channelId);
        Task DeleteChannelTopClipConfigAsync(decimal channelId, string broadcaster);
        Task<ChannelConfigContainer> GetChannelConfigAsync(decimal channelId);
        Task<List<PendingChannelTopClipConfig>> GetChannelTopClipConfigsAsync();
        Task<List<TopClipHistoryContainer>> InsertTopClipHistoriesAsync(List<TopClipHistoryContainer> containers);
        Task<ChannelConfigContainer> SetChannelConfigAsync(decimal channelId, ChannelConfigContainer container);
        Task<ChannelTopClipConfigContainer> SetChannelTopClipConfigAsync(decimal channelId, string broadcaster, ChannelTopClipConfigContainer container);
    }
}