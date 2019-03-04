using System.Collections.Generic;

namespace TopTwitchClipBotModel
{
    public class PendingChannelTopClipConfig
    {
        public decimal ChannelId { get; set; }
        public string Broadcaster { get; set; }
        public int? NumberOfClipsPerDay { get; set; }
        public List<string> ExistingSlugs { get; set; }
    }
}
