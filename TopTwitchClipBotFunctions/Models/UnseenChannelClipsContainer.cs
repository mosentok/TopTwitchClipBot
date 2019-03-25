using System.Collections.Generic;

namespace TopTwitchClipBotFunctions.Models
{
    public class UnseenChannelClipsContainer
    {
        public int? NumberOfClipsAtATime { get; set; }
        public List<ClipHistoryContainer> UnseenClips { get; set; }
        public UnseenChannelClipsContainer() { }
        public UnseenChannelClipsContainer(int? numberOfClipsAtATime, List<ClipHistoryContainer> unseenClips)
        {
            NumberOfClipsAtATime = numberOfClipsAtATime;
            UnseenClips = unseenClips;
        }
    }
}
