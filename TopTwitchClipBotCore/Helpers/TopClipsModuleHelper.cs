using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using TopTwitchClipBotCore.Enums;
using TopTwitchClipBotCore.Exceptions;
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
            string output;
            if (container.MinPostingHour.HasValue && container.MaxPostingHour.HasValue)
                output = $"between {container.MinPostingHour.Value} and {container.MaxPostingHour.Value}";
            else
                output = "all the time";
            var newLineDelimiter = _ConfigWrapper["NewLineDelimiter"];
            var postWhenFormat = _ConfigWrapper["PostWhenFormat"].Replace(newLineDelimiter, "\n");
            return string.Format(postWhenFormat, output);
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
            string output;
            if (container.NumberOfClipsAtATime.HasValue)
            {
                if (container.NumberOfClipsAtATime.Value == 1)
                    output = $"{container.NumberOfClipsAtATime.Value} clip at a time";
                else
                    output = $"{container.NumberOfClipsAtATime.Value} clips at a time";
            }
            else
                output = "no limit";
            var newLineDelimiter = _ConfigWrapper["NewLineDelimiter"];
            var clipsAtATimeFormat = _ConfigWrapper["ClipsAtATimeFormat"].Replace(newLineDelimiter, "\n");
            return string.Format(clipsAtATimeFormat, output);
        }
        public long? TicksFromIntervalTime(int interval, Time time)
        {
            if (interval < 1)
                return null;
            switch (time)
            {
                case Time.Minute:
                case Time.Minutes:
                    return TimeSpan.FromMinutes(interval).Ticks;
                case Time.Hour:
                case Time.Hours:
                    return TimeSpan.FromHours(interval).Ticks;
                default:
                    throw new ModuleException($"Invalid time '{time.ToString()}'.");
            }
        }
        public string TimeSpanBetweenClipsAsString(ChannelConfigContainer result)
        {
            string output;
            if (!result.TimeSpanBetweenClipsAsTicks.HasValue)
                output = "at least a few minutes";
            else
            {
                var timeSpan = TimeSpan.FromTicks(result.TimeSpanBetweenClipsAsTicks.Value);
                if (timeSpan.TotalMinutes == 1)
                    output = $"at least 1 minute";
                else if (timeSpan.TotalMinutes < 60)
                    output = $"at least {(int)timeSpan.TotalMinutes} minutes";
                else if (timeSpan.TotalHours == 1)
                    output = "at least 1 hour";
                else
                    output = $"at least {(int)timeSpan.TotalHours} hours";
            }
            var newLineDelimiter = _ConfigWrapper["NewLineDelimiter"];
            var timeBetweenClipsFormat = _ConfigWrapper["TimeBetweenClipsFormat"].Replace(newLineDelimiter, "\n");
            return string.Format(timeBetweenClipsFormat, output);
        }
        public Embed BuildChannelConfigEmbed(ICommandContext context, string postWhen, string streamersText, string clipsAtATime, string timeSpanString)
        {
            return new EmbedBuilder()
                .WithAuthor($"Setup for Channel # {context.Channel.Name}", context.Guild.IconUrl)
                .AddField("Post When?", postWhen, true)
                .AddField("Time Between Clips?", timeSpanString, true)
                .AddField("Clips at a Time", clipsAtATime, true)
                .AddField("Streamers", streamersText)
                .AddField("Need Help?", _ConfigWrapper["HelpQuestionFieldText"])
                .Build();
        }
    }
}
