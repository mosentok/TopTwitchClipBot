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
        public List<BroadcasterConfigContainer> Broadcasters { get; set; }
        public ChannelConfigContainer() { }
        public ChannelConfigContainer(ChannelConfig channelConfig)
        {
            ChannelId = channelConfig.ChannelId;
            Prefix = channelConfig.Prefix;
            MinPostingHour = channelConfig.MinPostingHour;
            MaxPostingHour = channelConfig.MaxPostingHour;
            Broadcasters = channelConfig.BroadcasterConfigs.Select(s => new BroadcasterConfigContainer(s)).ToList();
        }
        public ChannelConfigContainer(ChannelConfigContainer basedOn, int? minPostingHour, int? maxPostingHour)
        {
            ChannelId = basedOn.ChannelId;
            Prefix = basedOn.Prefix;
            MinPostingHour = minPostingHour;
            MaxPostingHour = maxPostingHour;
        }
    }
}
