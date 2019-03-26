using Discord;
using System.Collections.Generic;

namespace TopTwitchClipBotFunctions.Models
{
    public class ChannelContainer
    {
        public List<ClipHistoryContainer> Inserted { get; set; }
        public IMessageChannel Channel { get; set; }
        public ChannelContainer(List<ClipHistoryContainer> inserted, IMessageChannel channel)
        {
            Inserted = inserted;
            Channel = channel;
        }
    }
}
