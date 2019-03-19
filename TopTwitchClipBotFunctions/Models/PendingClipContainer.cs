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
        public int? GlobalMinViews { get; set; }
        public int? MinViews { get; set; }
        public List<BroadcasterHistoryContainer> ExistingHistories { get; set; }
        public List<Clip> Clips { get; set; }
        public PendingClipContainer() { }
        public PendingClipContainer(PendingBroadcasterConfig pendingBroadcasterConfig, GetClipsResponse getClipsResponse, int? globalMinViews)
        {
            Id = pendingBroadcasterConfig.Id;
            ChannelId = pendingBroadcasterConfig.ChannelId;
            Broadcaster = pendingBroadcasterConfig.Broadcaster;
            NumberOfClipsPerDay = pendingBroadcasterConfig.NumberOfClipsPerDay;
            GlobalMinViews = globalMinViews;
            MinViews = pendingBroadcasterConfig.MinViews;
            ExistingHistories = pendingBroadcasterConfig.ExistingHistories;
            Clips = getClipsResponse.Clips;
        }
        public PendingClipContainer FromClips(List<Clip> clips)
        {
            return new PendingClipContainer
            {
                Id = Id,
                ChannelId = ChannelId,
                Broadcaster = Broadcaster,
                NumberOfClipsPerDay = NumberOfClipsPerDay,
                MinViews = MinViews,
                ExistingHistories = ExistingHistories,
                Clips = clips
            };
        }
    }
}
