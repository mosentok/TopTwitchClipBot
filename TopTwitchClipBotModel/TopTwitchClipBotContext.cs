﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TopTwitchClipBotModel
{
    public partial class TopTwitchClipBotContext : DbContext, ITopTwitchClipBotContext
    {
        public virtual DbSet<ChannelConfig> ChannelConfigs { get; set; }
        public virtual DbSet<BroadcasterConfig> BroadcasterConfigs { get; set; }
        public virtual DbSet<BroadcasterHistory> BroadcasterHistories { get; set; }
        public TopTwitchClipBotContext() { }
        public TopTwitchClipBotContext(DbContextOptions<TopTwitchClipBotContext> options) : base(options) { }
        //TODO remove hardcoded command timeout
        public TopTwitchClipBotContext(string connectionString) : this(new DbContextOptionsBuilder<TopTwitchClipBotContext>()
            .UseSqlServer(connectionString, options => options.CommandTimeout((int)TimeSpan.FromMinutes(10).TotalSeconds))
            .UseLazyLoadingProxies().Options)
        { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
                entity.Relational().TableName = entity.DisplayName();
        }
        public async Task<ChannelConfigContainer> GetChannelConfigAsync(decimal channelId)
        {
            var match = await ChannelConfigs.SingleOrDefaultAsync(s => s.ChannelId == channelId);
            if (match != null)
                return new ChannelConfigContainer(match);
            return new ChannelConfigContainer { ChannelId = channelId, Broadcasters = new List<BroadcasterConfigContainer>() };
        }
        public async Task<ChannelConfigContainer> SetChannelConfigAsync(decimal channelId, ChannelConfigContainer container)
        {
            var match = await ChannelConfigs.SingleOrDefaultAsync(s => s.ChannelId == channelId);
            ChannelConfig channelConfig;
            if (match != null) //mutate it
            {
                channelConfig = match;
                channelConfig.Prefix = container.Prefix;
                channelConfig.MinPostingHour = container.MinPostingHour;
                channelConfig.MaxPostingHour = container.MaxPostingHour;
                channelConfig.NumberOfClipsAtATime = container.NumberOfClipsAtATime;
                channelConfig.TimeSpanBetweenClipsAsTicks = container.TimeSpanBetweenClipsAsTicks;
                channelConfig.GlobalMinViews = container.GlobalMinViews;
                channelConfig.UtcHourOffset = container.UtcHourOffset;
                channelConfig.ClipOrder = container.ClipOrder;
            }
            else
            {
                channelConfig = new ChannelConfig
                {
                    Prefix = container.Prefix,
                    MinPostingHour = container.MinPostingHour,
                    MaxPostingHour = container.MaxPostingHour,
                    NumberOfClipsAtATime = container.NumberOfClipsAtATime,
                    TimeSpanBetweenClipsAsTicks = container.TimeSpanBetweenClipsAsTicks,
                    GlobalMinViews = container.GlobalMinViews,
                    UtcHourOffset = container.UtcHourOffset,
                    ClipOrder = container.ClipOrder
                };
                ChannelConfigs.Add(channelConfig);
            }
            await SaveChangesAsync();
            return new ChannelConfigContainer(channelConfig);
        }
        public async Task<BroadcasterConfigContainer> GetBroadcasterConfigAsync(decimal channelId, string broadcaster)
        {
            var match = await BroadcasterConfigs.SingleOrDefaultAsync(s => s.ChannelId == channelId && s.Broadcaster == broadcaster);
            if (match != null)
                return new BroadcasterConfigContainer(match);
            return new BroadcasterConfigContainer { ChannelId = channelId, Broadcaster = broadcaster };
        }
        public async Task<ChannelConfigContainer> SetBroadcasterConfigAsync(decimal channelId, string displayName, BroadcasterConfigContainer container)
        {
            var match = await BroadcasterConfigs.SingleOrDefaultAsync(s => s.ChannelId == channelId && s.Broadcaster == displayName);
            BroadcasterConfig broadcasterConfig;
            if (match != null) //mutate it
            {
                broadcasterConfig = match;
                broadcasterConfig.Broadcaster = displayName;
                broadcasterConfig.NumberOfClipsPerDay = container.NumberOfClipsPerDay;
                broadcasterConfig.MinViews = container.MinViews;
            }
            else
            {
                broadcasterConfig = new BroadcasterConfig
                {
                    ChannelId = container.ChannelId,
                    Broadcaster = displayName,
                    NumberOfClipsPerDay = container.NumberOfClipsPerDay,
                    MinViews = container.MinViews
                };
                ChannelConfig channelConfig;
                var parent = await ChannelConfigs.SingleOrDefaultAsync(s => s.ChannelId == channelId);
                if (parent != null)
                {
                    channelConfig = parent;
                    channelConfig.BroadcasterConfigs.Add(broadcasterConfig);
                }
                else
                {
                    channelConfig = new ChannelConfig { ChannelId = channelId, BroadcasterConfigs = new List<BroadcasterConfig> { broadcasterConfig } };
                    ChannelConfigs.Add(channelConfig);
                }
            }
            await SaveChangesAsync();
            return new ChannelConfigContainer(broadcasterConfig.ChannelConfig);
        }
        public async Task<List<BroadcasterHistoryContainer>> InsertBroadcasterHistoriesAsync(List<BroadcasterHistoryContainer> containers)
        {
            var histories = containers.Select(container => new BroadcasterHistory
            {
                BroadcasterConfigId = container.BroadcasterConfigId,
                ClipUrl = container.ClipUrl,
                Slug = container.Slug,
                Stamp = container.Stamp
            }).ToList();
            BroadcasterHistories.AddRange(histories);
            await SaveChangesAsync();
            return containers;
        }
        public async Task<List<PendingChannelConfigContainer>> GetPendingChannelConfigsAsync(int nowUtcHour)
        {
            return await (from s in ChannelConfigs
                          let offsetHour = s.UtcHourOffset.HasValue ? //if they set an offset
                            (nowUtcHour + s.UtcHourOffset.Value + 24) % 24 : //add offset, in case that's negative add 24, altogether mod 24
                            nowUtcHour //else no offset to use
                          where s.MinPostingHour == null || s.MaxPostingHour == null ||
                               (s.MinPostingHour < s.MaxPostingHour && //if the range spans inside a single day
                                s.MinPostingHour <= offsetHour && offsetHour < s.MaxPostingHour) || //then now hour must be between at least min and less than max
                               (s.MinPostingHour > s.MaxPostingHour && //else if the range spans overnight into next day
                               (s.MinPostingHour <= offsetHour || offsetHour < s.MaxPostingHour)) //then now must be at least min or less than max
                          select new PendingChannelConfigContainer
                          {
                              ChannelId = s.ChannelId,
                              Prefix = s.Prefix,
                              MinPostingHour = s.MinPostingHour,
                              MaxPostingHour = s.MaxPostingHour,
                              NumberOfClipsAtATime = s.NumberOfClipsAtATime,
                              TimeSpanBetweenClipsAsTicks = s.TimeSpanBetweenClipsAsTicks,
                              GlobalMinViews = s.GlobalMinViews,
                              UtcHourOffset = s.UtcHourOffset,
                              ClipOrder = s.ClipOrder,
                              Broadcasters = s.BroadcasterConfigs.Select(t => new PendingBroadcasterConfig
                              {
                                  Id = t.Id,
                                  Broadcaster = t.Broadcaster,
                                  ChannelId = t.ChannelId,
                                  NumberOfClipsPerDay = t.NumberOfClipsPerDay,
                                  MinViews = t.MinViews,
                                  ExistingHistories = t.BroadcasterHistories.Select(u => new BroadcasterHistoryContainer(t.ChannelId, u)).ToList()
                              }).ToList()
                          }).ToListAsync();
        }
        public async Task DeleteBroadcasterConfigAsync(decimal channelId)
        {
            await Database.ExecuteSqlCommandAsync($"DELETE FROM BroadcasterConfig WHERE ChannelId = {channelId}");
        }
        public async Task DeleteBroadcasterConfigAsync(decimal channelId, string broadcaster)
        {
            await Database.ExecuteSqlCommandAsync($"DELETE FROM BroadcasterConfig WHERE ChannelId = {channelId} AND Broadcaster = {broadcaster}");
        }
    }
}
