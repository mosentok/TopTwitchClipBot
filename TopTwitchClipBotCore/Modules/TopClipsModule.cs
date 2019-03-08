using Discord.Commands;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using TopTwitchClipBotCore.Enums;
using TopTwitchClipBotCore.Exceptions;
using TopTwitchClipBotCore.Helpers;
using TopTwitchClipBotCore.Wrappers;
using TopTwitchClipBotModel;

namespace TopTwitchClipBotCore.Modules
{
    [Group("TopClips")]
    public class TopClipsModule : ModuleBase
    {
        readonly ITopClipsModuleHelper _TopClipsModuleHelper;
        readonly IFunctionWrapper _FunctionWrapper;
        readonly ILogger<TopClipsModule> _Log;
        public TopClipsModule(ITopClipsModuleHelper topClipsModuleHelper, IFunctionWrapper functionWrapper, ILogger<TopClipsModule> log)
        {
            _TopClipsModuleHelper = topClipsModuleHelper;
            _FunctionWrapper = functionWrapper;
            _Log = log;
        }
        [Command(nameof(Get))]
        [Alias(nameof(Get), "", "Config", "Setup")]
        public async Task Get()
        {
            var result = await _FunctionWrapper.GetChannelConfigAsync(Context.Channel.Id);
            await ReplyAsync(result);
        }
        [Command(nameof(Between))]
        public async Task Between([Remainder] string input)
        {
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
        public async Task Of(string broadcaster, int? numberOfClipsPerDay = null)
        {
            var container = new BroadcasterConfigContainer
            {
                ChannelId = Context.Channel.Id,
                Broadcaster = broadcaster,
                NumberOfClipsPerDay = numberOfClipsPerDay
            };
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
        [Command("At Least")]
        public async Task AtLeast(int interval, Time time)
        {
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
                await ReplyAsync(message: ex.Message);
            }
        }
        //TODO need to allow user to reset this to null
        [Command("At A Time")]
        public async Task AtATime(int numberOfClipsAtATime)
        {
            var match = await _FunctionWrapper.GetChannelConfigAsync(Context.Channel.Id);
            var container = match.FromClipsAtATime(numberOfClipsAtATime);
            var result = await _FunctionWrapper.PostChannelConfigAsync(Context.Channel.Id, container);
            await ReplyAsync(result);
        }
        async Task ReplyAsync(ChannelConfigContainer result)
        {
            var streamersText = _TopClipsModuleHelper.BuildStreamersText(result);
            var postWhen = _TopClipsModuleHelper.DeterminePostWhen(result);
            var clipsAtATime = _TopClipsModuleHelper.DetermineClipsAtATime(result);
            var timeSpanString = _TopClipsModuleHelper.TimeSpanBetweenClipsAsString(result);
            var embed = _TopClipsModuleHelper.BuildChannelConfigEmbed(Context, postWhen, streamersText, clipsAtATime, timeSpanString);
            await ReplyAsync(message: string.Empty, embed: embed);
        }
    }
}
