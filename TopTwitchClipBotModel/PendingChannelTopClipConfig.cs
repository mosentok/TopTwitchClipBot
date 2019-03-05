using System.Collections.Generic;

namespace TopTwitchClipBotModel
{
    public class PendingChannelTopClipConfig
    {
        public int Id { get; set; }
        public decimal ChannelId { get; set; }
        public string Broadcaster { get; set; }
        public int? NumberOfClipsPerDay { get; set; }
        public List<TopClipHistoryContainer> ExistingHistories { get; set; }
    }
}
