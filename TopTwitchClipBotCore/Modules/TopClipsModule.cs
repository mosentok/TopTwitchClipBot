using Discord;
using Discord.Commands;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using TopTwitchClipBotCore.Attributes;
using TopTwitchClipBotCore.Enums;
using TopTwitchClipBotCore.Exceptions;
using TopTwitchClipBotCore.Helpers;
using TopTwitchClipBotCore.Wrappers;
using TopTwitchClipBotModel;

namespace TopTwitchClipBotCore.Modules
{
    [Group("Top Clips")]
    public class TopClipsModule : ModuleBase
    {
        readonly ITopClipsModuleHelper _TopClipsModuleHelper;
        readonly IFunctionWrapper _FunctionWrapper;
        readonly ILogger<TopClipsModule> _Log;
        readonly IConfigurationWrapper _ConfigWrapper;
        public TopClipsModule(ITopClipsModuleHelper topClipsModuleHelper, IFunctionWrapper functionWrapper, ILogger<TopClipsModule> log, IConfigurationWrapper configWrapper)
        {
            _TopClipsModuleHelper = topClipsModuleHelper;
            _FunctionWrapper = functionWrapper;
            _Log = log;
            _ConfigWrapper = configWrapper;
        }
        [Command(nameof(Get))]
        [Alias("", "Config", "Setup")]
        public async Task Get()
        {
            var result = await _FunctionWrapper.GetChannelConfigAsync(Context.Channel.Id);
            await ReplyAsync(result);
        }
        [Command("Post When")]
        [Alias("Post Between")]
        public async Task PostWhen([Remainder] string input)
        {
            //TODO move "off" logic to another command
            var shouldTurnCommandOff = _TopClipsModuleHelper.ShouldTurnCommandOff(input);
            if (shouldTurnCommandOff)
            {
                var match = await _FunctionWrapper.GetChannelConfigAsync(Context.Channel.Id);
                var container = match.FromPostingHours(null, null);
                var result = await _FunctionWrapper.PostChannelConfigAsync(Context.Channel.Id, container);
                await ReplyAsync(result);
            }
            else
            {
                var isCorrectBetweenLength = _TopClipsModuleHelper.IsCorrectBetweenLength(input, out var split);
                if (!isCorrectBetweenLength)
                    return;
                var minInRange = _TopClipsModuleHelper.IsInRange(split[0], out var minPostingHour);
                var maxInRange = _TopClipsModuleHelper.IsInRange(split[2], out var maxPostingHour);
                if (!(minInRange && maxInRange) || minPostingHour == maxPostingHour)
                    return;
                var match = await _FunctionWrapper.GetChannelConfigAsync(Context.Channel.Id);
                var container = match.FromPostingHours(minPostingHour, maxPostingHour);
                var result = await _FunctionWrapper.PostChannelConfigAsync(Context.Channel.Id, container);
                await ReplyAsync(result);
            }
        }
        [Command(nameof(Of))]
        public async Task Of(string broadcaster, int? input = null, [Remainder] string option = null)
        {
            var match = await _FunctionWrapper.GetBroadcasterConfigAsync(Context.Channel.Id, broadcaster);
            BroadcasterConfigContainer container;
            var enableNumberOfClipsPerDay = _ConfigWrapper.GetValue<bool>("EnableNumberOfClipsPerDay");
            if (!enableNumberOfClipsPerDay) //default to min views logic
                container = ContainerFromMinViews();
            else if (string.IsNullOrEmpty(option)) //no default to assume, just return
                return;
            else
                switch (option.ToLower())
                {
                    case "clips":
                    case "clips per day":
                        if (input.HasValue && input.Value > 0)
                            container = match.FromClipsPerDay(input);
                        else
                            container = match.FromClipsPerDay(null);
                        break;
                    case "views":
                    case "min views":
                        container = ContainerFromMinViews();
                        break;
                    default:
                        container = new BroadcasterConfigContainer
                        {
                            ChannelId = Context.Channel.Id,
                            Broadcaster = broadcaster
                        };
                        break;
                }
            BroadcasterConfigContainer ContainerFromMinViews()
            {
                if (input.HasValue && input.Value > 0)
                    return match.FromMinViews(input);
                return match.FromMinViews(null);
            }
            var result = await _FunctionWrapper.PostBroadcasterConfigAsync(Context.Channel.Id, broadcaster, container);
            if (!string.IsNullOrEmpty(result.ErrorMessage))
                await ReplyAsync(message: result.ErrorMessage);
            else
                await ReplyAsync(result.ChannelConfigContainer);
        }
        [Command(nameof(Remove))]
        public async Task Remove(string broadcaster)
        {
            var shouldDeleteAll = broadcaster.Equals("all", StringComparison.CurrentCultureIgnoreCase) || broadcaster.Equals("all broadcasters", StringComparison.CurrentCultureIgnoreCase);
            ChannelConfigContainer result;
            if (shouldDeleteAll)
                result = await _FunctionWrapper.DeleteChannelTopClipConfigAsync(Context.Channel.Id);
            else
                result = await _FunctionWrapper.DeleteChannelTopClipConfigAsync(Context.Channel.Id, broadcaster);
            await ReplyAsync(result);
        }
        [Command("Time Between Clips")]
        public async Task TimeBetweenClips(int interval, Time time)
        {
            //TODO remove try/catch because time param must be valid for Discord.Net to invoke this method
            try
            {
                var ticks = _TopClipsModuleHelper.TicksFromIntervalTime(interval, time);
                var match = await _FunctionWrapper.GetChannelConfigAsync(Context.Channel.Id);
                var container = match.FromTimeSpanBetweenClipsAsTicks(ticks);
                var result = await _FunctionWrapper.PostChannelConfigAsync(Context.Channel.Id, container);
                await ReplyAsync(result);
            }
            catch (ModuleException ex)
            {
                _Log.LogError(ex, $"Error setting channel ID '{Context.Channel.Id}' interval '{interval}' time '{time.ToString()}'.");
            }
        }
        [Command("At A Time")]
        [Alias("Clips At A Time")]
        public async Task AtATime(int numberOfClipsAtATime)
        {
            var match = await _FunctionWrapper.GetChannelConfigAsync(Context.Channel.Id);
            ChannelConfigContainer container;
            if (numberOfClipsAtATime > 0)
                container = match.FromClipsAtATime(numberOfClipsAtATime);
            else
                container = match.FromClipsAtATime(null);
            var result = await _FunctionWrapper.PostChannelConfigAsync(Context.Channel.Id, container);
            await ReplyAsync(result);
        }
        [Command("Min Views")]
        public async Task MinViews(int minViews)
        {
            var match = await _FunctionWrapper.GetChannelConfigAsync(Context.Channel.Id);
            ChannelConfigContainer container;
            if (minViews > 0)
                container = match.FromGlobalMinViews(minViews);
            else
                container = match.FromGlobalMinViews(null);
            var result = await _FunctionWrapper.PostChannelConfigAsync(Context.Channel.Id, container);
            await ReplyAsync(result);
        }
        [Command(nameof(TimeZone))]
        [Alias("Time Zone", "Utc Offset", "UtcOffset")]
        public async Task TimeZone([ValidUtcOffset] decimal utcHourOffset)
        {
            var match = await _FunctionWrapper.GetChannelConfigAsync(Context.Channel.Id);
            var container = match.FromUtcHourOffset(utcHourOffset);
            var result = await _FunctionWrapper.PostChannelConfigAsync(Context.Channel.Id, container);
            await ReplyAsync(result);
        }
        async Task ReplyAsync(ChannelConfigContainer result)
        {
            var streamersText = _TopClipsModuleHelper.BuildStreamersText(result);
            var postWhen = _TopClipsModuleHelper.DeterminePostWhen(result);
            var clipsAtATime = _TopClipsModuleHelper.DetermineClipsAtATime(result);
            var timeSpanString = _TopClipsModuleHelper.TimeSpanBetweenClipsAsString(result);
            var globalMinViewsString = _TopClipsModuleHelper.GlobalMinViewsAsString(result);
            var timeZoneString = _TopClipsModuleHelper.BuildTimeZoneString(result);
            var embed = _TopClipsModuleHelper.BuildChannelConfigEmbed(Context, postWhen, streamersText, clipsAtATime, timeSpanString, globalMinViewsString, timeZoneString);
            await ReplyAsync(message: string.Empty, embed: embed);
        }
        protected override async Task<IUserMessage> ReplyAsync(string message = null, bool isTTS = false, Embed embed = null, RequestOptions options = null)
        {
            try
            {
                return await base.ReplyAsync(message, isTTS, embed, options);
            }
            catch (Exception ex)
            {
                _Log.LogError(ex, $"Error replying to channel {Context.Channel.Id}.");
            }
            return null;
        }
    }
}
