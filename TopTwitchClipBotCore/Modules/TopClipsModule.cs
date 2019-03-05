using Discord.Commands;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using TopTwitchClipBotCore.Wrappers;
using TopTwitchClipBotModel;

namespace TopTwitchClipBotCore.Modules
{
    [Group("TopClips")]
    public class TopClipsModule : ModuleBase
    {
        readonly IFunctionWrapper _FunctionWrapper;
        readonly ILogger<TopClipsModule> _Log;
        public TopClipsModule(IFunctionWrapper functionWrapper, ILogger<TopClipsModule> log)
        {
            _FunctionWrapper = functionWrapper;
            _Log = log;
        }
        [Command(nameof(Get))]
        [Alias(nameof(Get), "", "Config", "Setup")]
        public async Task Get()
        {
            var result = await _FunctionWrapper.GetChannelConfigAsync(Context.Channel.Id);
            //TODO embed builder
            var serialized = JsonConvert.SerializeObject(result);
            await ReplyAsync(serialized);
        }
        [Command(nameof(Between))]
        public async Task Between([Remainder] string input)
        {
            var split = input.Split(' ');
            if (split.Length != 3)
            {
                await ReplyAsync("TODO error bad input");
                return;
            }
            var minInRange = IsInRange(split[0], out var minPostingHour);
            var maxInRange = IsInRange(split[2], out var maxPostingHour);
            if (!(minInRange && maxInRange) || minPostingHour == maxPostingHour)
            {
                await ReplyAsync("TODO not in range message");
                return;
            }
            var match = await _FunctionWrapper.GetChannelConfigAsync(Context.Channel.Id);
            int? newMinPostingHour;
            int? newMaxPostingHour;
            var shouldTurnCommandOff = input.Equals("Off", StringComparison.CurrentCultureIgnoreCase) || input.Equals("all the time", StringComparison.CurrentCultureIgnoreCase);
            if (shouldTurnCommandOff)
            {
                newMinPostingHour = null;
                newMaxPostingHour = null;
            }
            else
            {
                newMinPostingHour = minPostingHour;
                newMaxPostingHour = maxPostingHour;
            }
            var container = new ChannelConfigContainer(match, minPostingHour, maxPostingHour);
            var result = await _FunctionWrapper.PostChannelConfigAsync(Context.Channel.Id, container);
            //TODO embed builder
            var serialized = JsonConvert.SerializeObject(result);
            await ReplyAsync(serialized);
        }
        bool IsInRange(string input, out int postingHour)
        {
            var success = int.TryParse(input, out postingHour);
            return success && 0 <= postingHour && postingHour <= 23;
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
            //TODO embed builder
            var serialized = JsonConvert.SerializeObject(result);
            await ReplyAsync(serialized);
        }
        [Command(nameof(Remove))]
        public async Task Remove(string broadcaster)
        {
            var shouldDeleteAll = broadcaster.Equals("all", StringComparison.CurrentCultureIgnoreCase) || broadcaster.Equals("all broadcasters", StringComparison.CurrentCultureIgnoreCase);
            if (shouldDeleteAll)
                await _FunctionWrapper.DeleteChannelTopClipConfigAsync(Context.Channel.Id);
            else
                await _FunctionWrapper.DeleteChannelTopClipConfigAsync(Context.Channel.Id, broadcaster);
            await ReplyAsync("Done.");
        }
    }
}
