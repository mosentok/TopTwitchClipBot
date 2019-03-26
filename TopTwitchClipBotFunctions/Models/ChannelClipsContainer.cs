using System.Collections.Generic;
using TopTwitchClipBotModel;

namespace TopTwitchClipBotFunctions.Models
{
    public class ChannelClipsContainer
    {
        public PendingChannelConfigContainer PendingChannelConfigContainer { get; set; }
        public List<PendingClipContainer> PendingClipContainers { get; set; }
        public ChannelClipsContainer() { }
        public ChannelClipsContainer(PendingChannelConfigContainer pendingChannelConfigContainer, List<PendingClipContainer> pendingClipContainers)
        {
            PendingChannelConfigContainer = pendingChannelConfigContainer;
            PendingClipContainers = pendingClipContainers;
        }
    }
}
