using System.Collections.Generic;
using TopTwitchClipBotModel;

namespace TopTwitchClipBotFunctions.Models
{
    public class PendingClipContainer
    {
        public int Id { get; set; }
        public decimal ChannelId { get; set; }
        public string Broadcaster { get; set; }
        public int? NumberOfClipsPerDay { get; set; }
        public List<BroadcasterHistoryContainer> ExistingHistories { get; set; }
        public List<Clip> Clips { get; set; }
        public PendingClipContainer() { }
        public PendingClipContainer(PendingBroadcasterConfig pendingBroadcasterConfig, GetClipsResponse getClipsResponse)
        {
            Id = pendingBroadcasterConfig.Id;
            ChannelId = pendingBroadcasterConfig.ChannelId;
            Broadcaster = pendingBroadcasterConfig.Broadcaster;
            NumberOfClipsPerDay = pendingBroadcasterConfig.NumberOfClipsPerDay;
            ExistingHistories = pendingBroadcasterConfig.ExistingHistories;
            Clips = getClipsResponse.Clips;
        }
    }
}
