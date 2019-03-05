using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TopTwitchClipBotModel
{
    public partial class TopTwitchClipBotContext : DbContext
    {
        public virtual DbSet<ChannelConfig> ChannelConfigs { get; set; }
        public virtual DbSet<ChannelTopClipConfig> ChannelTopClipConfigs { get; set; }
        public virtual DbSet<TopClipHistory> TopClipHistories { get; set; }
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
            return new ChannelConfigContainer { ChannelId = channelId };
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
            }
            else
            {
                channelConfig = new ChannelConfig
                {
                    Prefix = container.Prefix,
                    MinPostingHour = container.MinPostingHour,
                    MaxPostingHour = container.MaxPostingHour
                };
                ChannelConfigs.Add(channelConfig);
            }
            await SaveChangesAsync();
            return new ChannelConfigContainer(channelConfig);
        }
        public async Task<ChannelTopClipConfigContainer> SetChannelTopClipConfigAsync(decimal channelId, string broadcaster, ChannelTopClipConfigContainer container)
        {
            var match = await ChannelTopClipConfigs.SingleOrDefaultAsync(s => s.ChannelId == channelId && s.Broadcaster == broadcaster);
            ChannelTopClipConfig channelTopClipConfig;
            if (match != null) //mutate it
            {
                channelTopClipConfig = match;
                channelTopClipConfig.NumberOfClipsPerDay = container.NumberOfClipsPerDay;
            }
            else
            {
                channelTopClipConfig = new ChannelTopClipConfig
                {
                    ChannelId = container.ChannelId,
                    Broadcaster = container.Broadcaster,
                    NumberOfClipsPerDay = container.NumberOfClipsPerDay
                };
                ChannelConfig channelConfig;
                var parent = await ChannelConfigs.SingleOrDefaultAsync(s => s.ChannelId == channelId);
                if (parent != null)
                {
                    channelConfig = parent;
                    channelConfig.ChannelTopClipConfigs.Add(channelTopClipConfig);
                }
                else
                {
                    channelConfig = new ChannelConfig { ChannelId = channelId, ChannelTopClipConfigs = new List<ChannelTopClipConfig> { channelTopClipConfig } };
                    ChannelConfigs.Add(channelConfig);
                }
            }
            await SaveChangesAsync();
            return new ChannelTopClipConfigContainer(channelTopClipConfig);
        }
        public async Task<List<TopClipHistoryContainer>> InsertTopClipHistoriesAsync(List<TopClipHistoryContainer> containers)
        {
            var topClipHistories = containers.Select(container => new TopClipHistory
            {
                ChannelTopClipConfigId = container.ChannelTopClipConfigId,
                ClipUrl = container.ClipUrl,
                Slug = container.Slug,
                Stamp = container.Stamp
            }).ToList();
            TopClipHistories.AddRange(topClipHistories);
            await SaveChangesAsync();
            return containers;
        }
        public async Task<List<PendingChannelTopClipConfig>> GetChannelTopClipConfigsAsync()
        {
            return await ChannelTopClipConfigs.Select(s => new PendingChannelTopClipConfig
            {
                Id = s.Id,
                ChannelId = s.ChannelId,
                Broadcaster = s.Broadcaster,
                NumberOfClipsPerDay = s.NumberOfClipsPerDay,
                ExistingHistories = s.TopClipHistories.Select(t => new TopClipHistoryContainer(s.ChannelId, t)).ToList()
            }).ToListAsync();
        }
        public async Task DeleteChannelTopClipConfigAsync(decimal channelId)
        {
            await Database.ExecuteSqlCommandAsync($"DELETE FROM ChannelTopClipConfig WHERE ChannelId = {channelId}");
        }
        public async Task DeleteChannelTopClipConfigAsync(decimal channelId, string broadcaster)
        {
            await Database.ExecuteSqlCommandAsync($"DELETE FROM ChannelTopClipConfig WHERE ChannelId = {channelId} AND Broadcaster = {broadcaster}");
        }
    }
}
