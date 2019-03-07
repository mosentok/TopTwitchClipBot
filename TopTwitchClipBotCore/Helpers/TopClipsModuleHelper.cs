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
        public Embed BuildChannelConfigEmbed(ChannelConfigContainer container, ICommandContext context)
        {
            var author = new EmbedAuthorBuilder()
                .WithName($"Setup for Channel # {context.Channel.Name}")
                .WithIconUrl(context.Guild.IconUrl);
            string postWhen;
            if (container.MinPostingHour.HasValue && container.MaxPostingHour.HasValue)
                postWhen = $"```between {container.MinPostingHour.Value} and {container.MaxPostingHour.Value}```";
            else
                postWhen = "```all the time```";
            string streamersText;
            if (container.Broadcasters.Any())
            {
                streamersText = _ConfigWrapper["StreamersFieldBeginText"];
                var streamersFormats = _ConfigWrapper.Get<List<string>>("StreamersFormats").Select(s => s.Replace(_ConfigWrapper["NewLineDelimiter"], "\n")).ToList();
                var index = 0;
                var orderedBroadcasters = container.Broadcasters.OrderBy(s => s.Broadcaster);
                foreach (var broadcaster in orderedBroadcasters)
                {
                    string streamerText;
                    if (broadcaster.NumberOfClipsPerDay.HasValue)
                        streamerText = $"{broadcaster.Broadcaster}, {broadcaster.NumberOfClipsPerDay.Value} clips per day";
                    else
                        streamerText = $"{broadcaster.Broadcaster}, no limit";
                    streamersText += string.Format(streamersFormats[index], streamerText);
                    index = (index + 1) % streamersFormats.Count;
                }
            }
            else
                streamersText = _ConfigWrapper["NoStreamersText"];
            return new EmbedBuilder()
                .WithAuthor(author)
                .AddField("Post When?", postWhen)
                .AddField("Streamers", streamersText)
                .AddField("Need Help?", _ConfigWrapper["HelpQuestionFieldText"])
                .Build();
        }
    }
}
