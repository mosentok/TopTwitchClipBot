using Discord;

namespace TopTwitchClipBotFunctions.Models
{
    public class ChannelContainer
    {
        public UnseenChannelClipsContainer UnseenChannelClipsContainer { get; set; }
        public IMessageChannel Channel { get; set; }
        public ChannelContainer(UnseenChannelClipsContainer unseenChannelClipsContainer, IMessageChannel channel)
        {
            UnseenChannelClipsContainer = unseenChannelClipsContainer;
            Channel = channel;
        }
    }
}
