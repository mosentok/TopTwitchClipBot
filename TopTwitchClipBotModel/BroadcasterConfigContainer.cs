namespace TopTwitchClipBotModel
{
    public class BroadcasterConfigContainer
    {
        public decimal ChannelId { get; set; }
        public string Broadcaster { get; set; }
        public int? NumberOfClipsPerDay { get; set; }
        public int? MinViews { get; set; }
        public BroadcasterConfigContainer() { }
        public BroadcasterConfigContainer(BroadcasterConfig broadcasterConfig)
        {
            ChannelId = broadcasterConfig.ChannelId;
            Broadcaster = broadcasterConfig.Broadcaster;
            NumberOfClipsPerDay = broadcasterConfig.NumberOfClipsPerDay;
            MinViews = broadcasterConfig.MinViews;
        }
    }
}
