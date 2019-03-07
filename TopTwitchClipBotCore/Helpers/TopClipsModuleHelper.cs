using Discord;
using System;
using TopTwitchClipBotModel;

namespace TopTwitchClipBotCore.Helpers
{
    public class TopClipsModuleHelper : ITopClipsModuleHelper
    {
        public bool ShouldTurnCommandOff(string input)
        {
            return input.Equals("Off", StringComparison.CurrentCultureIgnoreCase) || input.Equals("all the time", StringComparison.CurrentCultureIgnoreCase);
        }
        public bool IsCorrectBetweenLength(string input, out string[] split)
        {
            split = input.Split(' ');
            return split.Length == 3;
        }
        public bool IsInRange(string postingHourString, out int postingHour)
        {
            var success = int.TryParse(postingHourString, out postingHour);
            return success && 0 <= postingHour && postingHour <= 23;
        }
        public bool ShouldDeleteAll(string broadcaster)
        {
            return broadcaster.Equals("all", StringComparison.CurrentCultureIgnoreCase) || broadcaster.Equals("all broadcasters", StringComparison.CurrentCultureIgnoreCase);
        }
        public Embed BuildChannelConfigEmbed(ChannelConfigContainer container)
        {
            string postWhen;
            if (container.MinPostingHour.HasValue && container.MaxPostingHour.HasValue)
                postWhen = $"between {container.MinPostingHour.Value} and {container.MaxPostingHour.Value}";
            else
                postWhen = "all the time";
            var embedBuilder = new EmbedBuilder().AddField("Post When?", postWhen);
            foreach (var broadcaster in container.Broadcasters)
            {
                string clipsPerDay;
                if (broadcaster.NumberOfClipsPerDay.HasValue)
                    clipsPerDay = $"{broadcaster.Broadcaster}, {broadcaster.NumberOfClipsPerDay.Value} clips/day";
                else
                    clipsPerDay = broadcaster.Broadcaster;
                embedBuilder = embedBuilder.AddField("Broadcaster", clipsPerDay);
            }
            //TODO github link, and move to config
            embedBuilder.AddField("Need Help?", "Type `!topclips examples` for some examples, or visit GitHub for more info.");
            return embedBuilder.Build();
        }
    }
}
