namespace TopTwitchClipBotModel
{
    public class ChannelTopClipConfigContainer
    {
        public decimal ChannelId { get; set; }
        public string Broadcaster { get; set; }
        public int? NumberOfClipsPerDay { get; set; }
        public ChannelTopClipConfigContainer() { }
        public ChannelTopClipConfigContainer(ChannelTopClipConfig channelTopClipConfig)
        {
            ChannelId = channelTopClipConfig.ChannelId;
            Broadcaster = channelTopClipConfig.Broadcaster;
            NumberOfClipsPerDay = channelTopClipConfig.NumberOfClipsPerDay;
        }
    }
}
