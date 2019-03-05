using TopTwitchClipBotModel;

namespace TopTwitchClipBotFunctions.Models
{
    public class PendingClipContainer
    {
        public PendingBroadcasterConfig PendingBroadcasterConfig { get; set; }
        public GetClipsResponse GetClipsResponse { get; set; }
        public PendingClipContainer(PendingBroadcasterConfig pendingBroadcasterConfig, GetClipsResponse getClipsResponse)
        {
            PendingBroadcasterConfig = pendingBroadcasterConfig;
            GetClipsResponse = getClipsResponse;
        }
    }
}
