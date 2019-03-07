using Microsoft.EntityFrameworkCore;
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
        public async Task<ChannelConfigContainer> SetBroadcasterConfigAsync(decimal channelId, string broadcaster, BroadcasterConfigContainer container)
        {
            var match = await BroadcasterConfigs.SingleOrDefaultAsync(s => s.ChannelId == channelId && s.Broadcaster == broadcaster);
            BroadcasterConfig broadcasterConfig;
            if (match != null) //mutate it
            {
                broadcasterConfig = match;
                broadcasterConfig.NumberOfClipsPerDay = container.NumberOfClipsPerDay;
            }
            else
            {
                broadcasterConfig = new BroadcasterConfig
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
        public async Task<List<PendingBroadcasterConfig>> GetBroadcasterConfigsAsync()
        {
            return await BroadcasterConfigs.Select(s => new PendingBroadcasterConfig
            {
                Id = s.Id,
                ChannelId = s.ChannelId,
                Broadcaster = s.Broadcaster,
                NumberOfClipsPerDay = s.NumberOfClipsPerDay,
                ExistingHistories = s.BroadcasterHistories.Select(t => new BroadcasterHistoryContainer(s.ChannelId, t)).ToList()
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
