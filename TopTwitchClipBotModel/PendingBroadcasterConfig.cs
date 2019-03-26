using System.Collections.Generic;

namespace TopTwitchClipBotModel
{
    public class PendingBroadcasterConfig
    {
        public int Id { get; set; }
        public decimal ChannelId { get; set; }
        public string Broadcaster { get; set; }
        public int? NumberOfClipsPerDay { get; set; }
        public int? MinViews { get; set; }
        public List<BroadcasterHistoryContainer> ExistingHistories { get; set; }
    }
}
