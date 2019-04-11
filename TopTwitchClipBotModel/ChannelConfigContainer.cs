using System.Collections.Generic;
using System.Linq;

namespace TopTwitchClipBotModel
{
    public class ChannelConfigContainer
    {
        public decimal ChannelId { get; set; }
        public string Prefix { get; set; }
        public int? MinPostingHour { get; set; }
        public int? MaxPostingHour { get; set; }
        public int? NumberOfClipsAtATime { get; set; }
        public long? TimeSpanBetweenClipsAsTicks { get; set; }
        public int? GlobalMinViews { get; set; }
        public decimal? UtcHourOffset { get; set; }
        public string ClipOrder { get; set; }
        public List<BroadcasterConfigContainer> Broadcasters { get; set; }
        public ChannelConfigContainer() { }
        public ChannelConfigContainer(ChannelConfig channelConfig)
        {
            ChannelId = channelConfig.ChannelId;
            Prefix = channelConfig.Prefix;
            MinPostingHour = channelConfig.MinPostingHour;
            MaxPostingHour = channelConfig.MaxPostingHour;
            NumberOfClipsAtATime = channelConfig.NumberOfClipsAtATime;
            TimeSpanBetweenClipsAsTicks = channelConfig.TimeSpanBetweenClipsAsTicks;
            GlobalMinViews = channelConfig.GlobalMinViews;
            UtcHourOffset = channelConfig.UtcHourOffset;
            ClipOrder = channelConfig.ClipOrder;
            Broadcasters = channelConfig.BroadcasterConfigs.Select(s => new BroadcasterConfigContainer(s)).ToList();
        }
        public ChannelConfigContainer FromPostingHours(int? minPostingHour, int? maxPostingHour)
        {
            return new ChannelConfigContainer
            {
                ChannelId = ChannelId,
                Prefix = Prefix,
                MinPostingHour = minPostingHour,
                MaxPostingHour = maxPostingHour,
                NumberOfClipsAtATime = NumberOfClipsAtATime,
                TimeSpanBetweenClipsAsTicks = TimeSpanBetweenClipsAsTicks,
                GlobalMinViews = GlobalMinViews,
                UtcHourOffset = UtcHourOffset,
                ClipOrder = ClipOrder
            };
        }
        public ChannelConfigContainer FromClipsAtATime(int? numberOfClipsAtATime)
        {
            return new ChannelConfigContainer
            {
                ChannelId = ChannelId,
                Prefix = Prefix,
                MinPostingHour = MinPostingHour,
                MaxPostingHour = MaxPostingHour,
                NumberOfClipsAtATime = numberOfClipsAtATime,
                TimeSpanBetweenClipsAsTicks = TimeSpanBetweenClipsAsTicks,
                GlobalMinViews = GlobalMinViews,
                UtcHourOffset = UtcHourOffset,
                ClipOrder = ClipOrder
            };
        }
        public ChannelConfigContainer FromTimeSpanBetweenClipsAsTicks(long? timeSpanBetweenClipsAsTicks)
        {
            return new ChannelConfigContainer
            {
                ChannelId = ChannelId,
                Prefix = Prefix,
                MinPostingHour = MinPostingHour,
                MaxPostingHour = MaxPostingHour,
                NumberOfClipsAtATime = NumberOfClipsAtATime,
                TimeSpanBetweenClipsAsTicks = timeSpanBetweenClipsAsTicks,
                GlobalMinViews = GlobalMinViews,
                UtcHourOffset = UtcHourOffset,
                ClipOrder = ClipOrder
            };
        }
        public ChannelConfigContainer FromGlobalMinViews(int? globalMinViews)
        {
            return new ChannelConfigContainer
            {
                ChannelId = ChannelId,
                Prefix = Prefix,
                MinPostingHour = MinPostingHour,
                MaxPostingHour = MaxPostingHour,
                NumberOfClipsAtATime = NumberOfClipsAtATime,
                TimeSpanBetweenClipsAsTicks = TimeSpanBetweenClipsAsTicks,
                GlobalMinViews = globalMinViews,
                UtcHourOffset = UtcHourOffset,
                ClipOrder = ClipOrder
            };
        }
        public ChannelConfigContainer FromUtcHourOffset(decimal? utcHourOffset)
        {
            return new ChannelConfigContainer
            {
                ChannelId = ChannelId,
                Prefix = Prefix,
                MinPostingHour = MinPostingHour,
                MaxPostingHour = MaxPostingHour,
                NumberOfClipsAtATime = NumberOfClipsAtATime,
                TimeSpanBetweenClipsAsTicks = TimeSpanBetweenClipsAsTicks,
                GlobalMinViews = GlobalMinViews,
                UtcHourOffset = utcHourOffset,
                ClipOrder = ClipOrder
            };
        }
        public ChannelConfigContainer FromClipOrder(string clipOrder)
        {
            return new ChannelConfigContainer
            {
                ChannelId = ChannelId,
                Prefix = Prefix,
                MinPostingHour = MinPostingHour,
                MaxPostingHour = MaxPostingHour,
                NumberOfClipsAtATime = NumberOfClipsAtATime,
                TimeSpanBetweenClipsAsTicks = TimeSpanBetweenClipsAsTicks,
                GlobalMinViews = GlobalMinViews,
                UtcHourOffset = UtcHourOffset,
                ClipOrder = clipOrder
            };
        }
    }
}
