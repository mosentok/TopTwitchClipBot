using System.Collections.Generic;
using TopTwitchClipBotModel;

namespace TopTwitchClipBotFunctions.Models
{
    public class UnseenChannelClipsContainer
    {
        public PendingChannelConfigContainer PendingChannelConfigContainer { get; set; }
        public List<ClipHistoryContainer> UnseenClips { get; set; }
        public UnseenChannelClipsContainer() { }
        public UnseenChannelClipsContainer(PendingChannelConfigContainer pendingChannelConfigContainer, List<ClipHistoryContainer> unseenClips)
        {
            PendingChannelConfigContainer = pendingChannelConfigContainer;
            UnseenClips = unseenClips;
        }
    }
}
