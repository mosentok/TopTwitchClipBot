using Discord;
using System.Collections.Generic;

namespace TopTwitchClipBotFunctions.Models
{
    public class ChannelContainer
    {
        public List<InsertedBroadcasterHistoryContainer> Inserted { get; set; }
        public IMessageChannel Channel { get; set; }
        public ChannelContainer(List<InsertedBroadcasterHistoryContainer> inserted, IMessageChannel channel)
        {
            Inserted = inserted;
            Channel = channel;
        }
    }
}
