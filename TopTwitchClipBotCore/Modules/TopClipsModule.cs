using Discord.Commands;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Threading.Tasks;
using TopTwitchClipBotCore.Helpers;

namespace TopTwitchClipBotCore.Modules
{
    [Group("TopClips")]
    public class TopClipsModule : ModuleBase
    {
        readonly TopClipsModuleHelper _TopClipsModuleHelper;
        readonly ILogger<TopClipsModule> _Log;
        public TopClipsModule(TopClipsModuleHelper topClipsModuleHelper, ILogger<TopClipsModule> log)
        {
            _TopClipsModuleHelper = topClipsModuleHelper;
            _Log = log;
        }
        [Command(nameof(Get))]
        [Alias(nameof(Get), "", "Config", "Setup")]
        public async Task Get()
        {
            var result = await _TopClipsModuleHelper.GetAsync(Context.Channel.Id);
            var serialized = JsonConvert.SerializeObject(result);
            await ReplyAsync(serialized);
        }
        [Command(nameof(Between))]
        public async Task Between([Remainder] string input)
        {
            var shouldTurnCommandOff = _TopClipsModuleHelper.ShouldTurnCommandOff(input);
            if (shouldTurnCommandOff)
            {
                var result = await _TopClipsModuleHelper.BetweenAsync(input, Context.Channel.Id, null, null);
                //TODO embed builder
                var serialized = JsonConvert.SerializeObject(result);
                await ReplyAsync(serialized);
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
                var result = await _TopClipsModuleHelper.BetweenAsync(input, Context.Channel.Id, minPostingHour, maxPostingHour);
                //TODO embed builder
                var serialized = JsonConvert.SerializeObject(result);
                await ReplyAsync(serialized);
            }
        }
        [Command(nameof(Of))]
        public async Task Of(string broadcaster, int? numberOfClipsPerDay = null)
        {
            var result = await _TopClipsModuleHelper.OfAsync(Context.Channel.Id, broadcaster, numberOfClipsPerDay);
            //TODO embed builder
            var serialized = JsonConvert.SerializeObject(result);
            await ReplyAsync(serialized);
        }
        [Command(nameof(Remove))]
        public async Task Remove(string broadcaster)
        {
            var shouldDeleteAll = _TopClipsModuleHelper.ShouldDeleteAll(broadcaster);
            if (shouldDeleteAll)
                await _TopClipsModuleHelper.RemoveAsync(Context.Channel.Id);
            else
                await _TopClipsModuleHelper.RemoveAsync(Context.Channel.Id, broadcaster);
            await ReplyAsync("Done.");
        }
    }
}
