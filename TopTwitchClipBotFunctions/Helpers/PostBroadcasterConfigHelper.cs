using System.Threading.Tasks;
using TopTwitchClipBotFunctions.Wrappers;
using TopTwitchClipBotModel;

namespace TopTwitchClipBotFunctions.Helpers
{
    public class PostBroadcasterConfigHelper
    {
        readonly ILoggerWrapper _Log;
        readonly ITopTwitchClipBotContext _Context;
        public PostBroadcasterConfigHelper(ILoggerWrapper log, ITopTwitchClipBotContext context)
        {
            _Log = log;
            _Context = context;
        }
        public async Task<BroadcasterConfigContainer> PostBroadcasterConfigAsync(decimal channelId, string broadcaster, BroadcasterConfigContainer container)
        {
            _Log.LogInformation($"Posting broadcaster config for channel '{channelId}' broadcaster '{broadcaster}'.");
            var result = await _Context.SetBroadcasterConfigAsync(channelId, broadcaster, container);
            _Log.LogInformation($"Posted broadcaster config for channel '{channelId}' broadcaster '{broadcaster}'.");
            return result;
        }
    }
}
