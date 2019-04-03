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
        public bool IsInUtcRange(decimal utcHourOffset)
        {
            var min = _ConfigWrapper.GetValue<decimal>("UtcHourOffsetMin");
            var max = _ConfigWrapper.GetValue<decimal>("UtcHourOffsetMax");
            return min <= utcHourOffset && utcHourOffset <= max;
        }
        public bool IsValidTimeZoneFraction(decimal utcHourOffset)
        { 
            var validTimeZoneFractions = _ConfigWrapper.Get<List<decimal>>("ValidTimeZoneFractions");
            return utcHourOffset % 1 == 0 || validTimeZoneFractions.Any(s => utcHourOffset % s == 0);
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
            var enableNumberOfClipsPerDay = _ConfigWrapper.GetValue<bool>("EnableNumberOfClipsPerDay");
            var streamersText = _ConfigWrapper["StreamersFieldBeginText"];
            var newLineDelimiter = _ConfigWrapper["NewLineDelimiter"];
            var streamersFormats = _ConfigWrapper.Get<List<string>>("StreamersFormats").Select(s => s.Replace(newLineDelimiter, "\n")).ToList();
            var index = 0;
            var orderedBroadcasters = container.Broadcasters.OrderBy(s => s.Broadcaster);
            foreach (var broadcaster in orderedBroadcasters)
            {
                var broadcasterText = broadcaster.Broadcaster;
                var numberOfClipsPerDayText = DetermineNumberOfClipsPerDayText();
                var minViewsText = DetermineMinViewsText();
                var thisStreamersText = DetermineThisStreamersText();
                streamersText += string.Format(streamersFormats[index], thisStreamersText);
                index = (index + 1) % streamersFormats.Count;
                string DetermineNumberOfClipsPerDayText()
                {
                    if (!broadcaster.NumberOfClipsPerDay.HasValue)
                        return "no limit";
                    if (broadcaster.NumberOfClipsPerDay.Value == 1)
                        return "1 clip per day";
                    return $"{broadcaster.NumberOfClipsPerDay.Value} clips per day";
                }
                string DetermineMinViewsText()
                {
                    if (!broadcaster.MinViews.HasValue)
                    {
                        if (container.GlobalMinViews.HasValue)
                            return $"at least {container.GlobalMinViews.Value.ToString("N0")} views (global)";
                        return "any view count";
                    }
                    if (broadcaster.MinViews.Value == 1)
                        return "at least 1 view";
                    return $"at least {broadcaster.MinViews.Value.ToString("N0")} views";
                }
                string DetermineThisStreamersText()
                {
                    if (enableNumberOfClipsPerDay)
                        return string.Join(", ", broadcasterText, numberOfClipsPerDayText, minViewsText);
                    return string.Join(", ", broadcasterText, minViewsText);
                }
            }
            return streamersText;
        }
        public string DetermineClipsAtATime(ChannelConfigContainer container)
        {
            var output = DetermineClipsAtATimeText();
            string DetermineClipsAtATimeText()
            {
                if (!container.NumberOfClipsAtATime.HasValue)
                    return "no limit";
                if (container.NumberOfClipsAtATime.Value == 1)
                    return $"{container.NumberOfClipsAtATime.Value} clip at a time";
                return $"{container.NumberOfClipsAtATime.Value} clips at a time";
            }
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
            var output = DetermineTimeBetweenClipsText();
            string DetermineTimeBetweenClipsText()
            {
                if (!result.TimeSpanBetweenClipsAsTicks.HasValue)
                    return "at least a few minutes";
                var timeSpan = TimeSpan.FromTicks(result.TimeSpanBetweenClipsAsTicks.Value);
                if (timeSpan.TotalMinutes == 1)
                    return $"at least 1 minute";
                if (timeSpan.TotalMinutes < 60)
                    return $"at least {(int)timeSpan.TotalMinutes} minutes";
                if (timeSpan.TotalHours == 1)
                    return "at least 1 hour";
                return $"at least {(int)timeSpan.TotalHours} hours";
            }
            var newLineDelimiter = _ConfigWrapper["NewLineDelimiter"];
            var timeBetweenClipsFormat = _ConfigWrapper["TimeBetweenClipsFormat"].Replace(newLineDelimiter, "\n");
            return string.Format(timeBetweenClipsFormat, output);
        }
        public string GlobalMinViewsAsString(ChannelConfigContainer result)
        {
            string output;
            if (!result.GlobalMinViews.HasValue)
                output = "any view count";
            else
                output = $"at least {result.GlobalMinViews.Value.ToString("N0")} views";
            var newLineDelimiter = _ConfigWrapper["NewLineDelimiter"];
            var globalMinViewsFormat = _ConfigWrapper["GlobalMinViewsFormat"].Replace(newLineDelimiter, "\n");
            return string.Format(globalMinViewsFormat, output);
        }
        public string BuildTimeZoneString(ChannelConfigContainer result)
        {
            string output;
            if (!result.UtcHourOffset.HasValue)
                output = "none";
            else
            {
                var utcHourOffsetString = result.UtcHourOffset.Value.ToString("#.#");
                if (result.UtcHourOffset.Value >= 0)
                    output = $"UTC+{utcHourOffsetString}";
                else
                    output = $"UTC{utcHourOffsetString}";
            }
            var newLineDelimiter = _ConfigWrapper["NewLineDelimiter"];
            var timeZoneFormat = _ConfigWrapper["TimeZoneFormat"].Replace(newLineDelimiter, "\n");
            return string.Format(timeZoneFormat, output);
        }
        public Embed BuildChannelConfigEmbed(ICommandContext context, string postWhen, string streamersText, string clipsAtATime, string timeSpanString, string globalMinViewsString, string timeZoneString)
        {
            return new EmbedBuilder()
                .WithAuthor($"Setup for Channel # {context.Channel.Name}", context.Guild.IconUrl)
                .AddField("Post When?", postWhen, true)
                .AddField("Time Between Clips?", timeSpanString, true)
                .AddField("Clips at a Time", clipsAtATime, true)
                .AddField("Global Min Views?", globalMinViewsString, true)
                .AddField("Time Zone", timeZoneString, true)
                .AddField("Streamers", streamersText)
                .AddField("Need Help?", _ConfigWrapper["HelpQuestionFieldText"])
                .Build();
        }
    }
}
