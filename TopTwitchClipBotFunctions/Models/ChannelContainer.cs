using Discord;
using System.Collections.Generic;
using TopTwitchClipBotModel;

namespace TopTwitchClipBotFunctions.Models
{
    public class ChannelContainer
    {
        public List<BroadcasterHistoryContainer> BroadcasterHistoryContainers { get; set; }
        public IMessageChannel Channel { get; set; }
        public ChannelContainer(List<BroadcasterHistoryContainer> broadcasterHistoryContainers, IMessageChannel channel)
        {
            BroadcasterHistoryContainers = broadcasterHistoryContainers;
            Channel = channel;
        }
    }
}
