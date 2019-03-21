using System.Threading.Tasks;
using TopTwitchClipBotFunctions.Wrappers;
using TopTwitchClipBotModel;

namespace TopTwitchClipBotFunctions.Helpers
{
    public class GetBroadcasterConfigHelper
    {
        readonly ILoggerWrapper _Log;
        readonly ITopTwitchClipBotContext _Context;
        public GetBroadcasterConfigHelper(ILoggerWrapper log, ITopTwitchClipBotContext context)
        {
            _Log = log;
            _Context = context;
        }
        public async Task<BroadcasterConfigContainer> GetBroadcasterConfigAsync(decimal channelId, string broadcaster)
        {
            _Log.LogInformation($"Getting channel config for channel '{channelId}'.");
            var result = await _Context.GetBroadcasterConfigAsync(channelId, broadcaster);
            _Log.LogInformation($"Got channel config for channel '{channelId}'.");
            return result;
        }
    }
}
