using TopTwitchClipBotModel;

namespace TopTwitchClipBotCore.Models
{
    public class PostBroadcasterConfigResponse
    {
        public string ErrorMessage { get; set; }
        public ChannelConfigContainer ChannelConfigContainer { get; set; }
        public PostBroadcasterConfigResponse() { }
        public PostBroadcasterConfigResponse(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }
        public PostBroadcasterConfigResponse(ChannelConfigContainer channelConfigContainer)
        {
            ChannelConfigContainer = channelConfigContainer;
        }
    }
}
