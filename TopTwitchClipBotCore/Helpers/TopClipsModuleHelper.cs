using System;
using System.Threading.Tasks;
using TopTwitchClipBotCore.Wrappers;
using TopTwitchClipBotModel;

namespace TopTwitchClipBotCore.Helpers
{
    public class TopClipsModuleHelper
    {
        readonly IFunctionWrapper _FunctionWrapper;
        public TopClipsModuleHelper(IFunctionWrapper functionWrapper)
        {
            _FunctionWrapper = functionWrapper;
        }
        public async Task<ChannelConfigContainer> GetAsync(decimal channelId)
        {
            return await _FunctionWrapper.GetChannelConfigAsync(channelId);
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
        public async Task<ChannelConfigContainer> BetweenAsync(string input, decimal channelId, int? minPostingHour, int? maxPostingHour)
        {
            var match = await _FunctionWrapper.GetChannelConfigAsync(channelId);
            var container = new ChannelConfigContainer(match, minPostingHour, maxPostingHour);
            return await _FunctionWrapper.PostChannelConfigAsync(channelId, container);
        }
        public async Task<BroadcasterConfigContainer> OfAsync(decimal channelId, string broadcaster, int? numberOfClipsPerDay)
        {
            var container = new BroadcasterConfigContainer
            {
                ChannelId = channelId,
                Broadcaster = broadcaster,
                NumberOfClipsPerDay = numberOfClipsPerDay
            };
            return await _FunctionWrapper.PostBroadcasterConfigAsync(channelId, broadcaster, container);
        }
        public bool ShouldDeleteAll(string broadcaster)
        {
            return broadcaster.Equals("all", StringComparison.CurrentCultureIgnoreCase) || broadcaster.Equals("all broadcasters", StringComparison.CurrentCultureIgnoreCase);
        }
        public async Task RemoveAsync(decimal channelId)
        {
            await _FunctionWrapper.DeleteChannelTopClipConfigAsync(channelId);
        }
        public async Task RemoveAsync(decimal channelId, string broadcaster)
        {
            await _FunctionWrapper.DeleteChannelTopClipConfigAsync(channelId, broadcaster);
        }
    }
}
