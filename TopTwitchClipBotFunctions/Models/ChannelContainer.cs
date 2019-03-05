using Discord;
using System.Collections.Generic;
using TopTwitchClipBotModel;

namespace TopTwitchClipBotFunctions.Models
{
    public class ChannelContainer
    {
        public List<TopClipHistoryContainer> TopClipHistoryContainers { get; set; }
        public IMessageChannel Channel { get; set; }
        public ChannelContainer(List<TopClipHistoryContainer> topClipHistoryContainers, IMessageChannel channel)
        {
            TopClipHistoryContainers = topClipHistoryContainers;
            Channel = channel;
        }
    }
}
