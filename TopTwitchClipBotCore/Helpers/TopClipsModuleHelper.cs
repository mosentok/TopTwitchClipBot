using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using TopTwitchClipBotCore.Wrappers;
using TopTwitchClipBotModel;

namespace TopTwitchClipBotCore.Helpers
{
    public class TopClipsModuleHelper : ITopClipsModuleHelper
    {
        readonly IConfigurationWrapper _ConfigWrapper;
        public TopClipsModuleHelper(IConfigurationWrapper configWrapper)
        {
            _ConfigWrapper = configWrapper;
        }
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
        public string DeterminePostWhen(ChannelConfigContainer container)
        {
            if (container.MinPostingHour.HasValue && container.MaxPostingHour.HasValue)
                return $"```between {container.MinPostingHour.Value} and {container.MaxPostingHour.Value}```";
            return "```all the time```";
        }
        public string BuildStreamersText(ChannelConfigContainer container)
        {
            if (!container.Broadcasters.Any())
                return _ConfigWrapper["NoStreamersText"];
            var streamersText = _ConfigWrapper["StreamersFieldBeginText"];
            var newLineDelimiter = _ConfigWrapper["NewLineDelimiter"];
            var streamersFormats = _ConfigWrapper.Get<List<string>>("StreamersFormats").Select(s => s.Replace(newLineDelimiter, "\n")).ToList();
            var index = 0;
            var orderedBroadcasters = container.Broadcasters.OrderBy(s => s.Broadcaster);
            foreach (var broadcaster in orderedBroadcasters)
            {
                string streamerText;
                if (broadcaster.NumberOfClipsPerDay.HasValue)
                    if (broadcaster.NumberOfClipsPerDay.Value == 1)
                        streamerText = $"{broadcaster.Broadcaster}, {broadcaster.NumberOfClipsPerDay.Value} clip per day";
                    else
                        streamerText = $"{broadcaster.Broadcaster}, {broadcaster.NumberOfClipsPerDay.Value} clips per day";
                else
                    streamerText = $"{broadcaster.Broadcaster}, no limit";
                streamersText += string.Format(streamersFormats[index], streamerText);
                index = (index + 1) % streamersFormats.Count;
            }
            return streamersText;
        }
        public string DetermineClipsAtATime(ChannelConfigContainer container)
        {
            if (container.NumberOfClipsAtATime.HasValue)
                if (container.NumberOfClipsAtATime.Value == 1)
                    return $"```{container.NumberOfClipsAtATime.Value} clip at a time```";
                else
                    return $"```{container.NumberOfClipsAtATime.Value} clips at a time```";
            return "```no limit```";
        }
        public Embed BuildChannelConfigEmbed(ICommandContext context, string postWhen, string streamersText, string clipsAtATime)
        {
            return new EmbedBuilder()
                .WithAuthor($"Setup for Channel # {context.Channel.Name}", context.Guild.IconUrl)
                .AddField("Post When?", postWhen, true)
                .AddField("Clips at a Time", clipsAtATime, true)
                .AddField("Streamers", streamersText)
                .AddField("Need Help?", _ConfigWrapper["HelpQuestionFieldText"])
                .Build();
        }
    }
}
