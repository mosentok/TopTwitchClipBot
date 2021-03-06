﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace TopTwitchClipBotModel
{
    public interface ITopTwitchClipBotContext
    {
        DbSet<ChannelConfig> ChannelConfigs { get; set; }
        DbSet<BroadcasterConfig> BroadcasterConfigs { get; set; }
        DbSet<BroadcasterHistory> BroadcasterHistories { get; set; }

        Task DeleteBroadcasterConfigAsync(decimal channelId);
        Task DeleteBroadcasterConfigAsync(decimal channelId, string broadcaster);
        Task<ChannelConfigContainer> GetChannelConfigAsync(decimal channelId);
        Task<List<PendingChannelConfigContainer>> GetPendingChannelConfigsAsync(int nowUtcHour);
        Task<BroadcasterConfigContainer> GetBroadcasterConfigAsync(decimal channelId, string broadcaster);
        Task<List<BroadcasterHistoryContainer>> InsertBroadcasterHistoriesAsync(List<BroadcasterHistoryContainer> containers);
        Task<ChannelConfigContainer> SetChannelConfigAsync(decimal channelId, ChannelConfigContainer container);
        Task<ChannelConfigContainer> SetBroadcasterConfigAsync(decimal channelId, string displayName, BroadcasterConfigContainer container);
    }
}