using TopTwitchClipBotModel;

namespace TopTwitchClipBotFunctions.Models
{
    public class PendingClipContainer
    {
        public PendingChannelTopClipConfig PendingChannelTopClipConfig { get; set; }
        public GetClipsResponse GetClipsResponse { get; set; }
        public PendingClipContainer(PendingChannelTopClipConfig pendingChannelTopClipConfig, GetClipsResponse getClipsResponse)
        {
            PendingChannelTopClipConfig = pendingChannelTopClipConfig;
            GetClipsResponse = getClipsResponse;
        }
    }
}
