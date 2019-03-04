using Discord;
using TopTwitchClipBotModel;

namespace TopTwitchClipBotFunctions.Models
{
    public class ChannelContainer
    {
        public TopClipHistoryContainer TopClipHistoryContainer { get; set; }
        public IMessageChannel Channel { get; set; }
        public ChannelContainer(TopClipHistoryContainer topClipHistoryContainer, IMessageChannel channel)
        {
            TopClipHistoryContainer = topClipHistoryContainer;
            Channel = channel;
        }
    }
}
