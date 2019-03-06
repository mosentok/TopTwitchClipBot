using System.Threading.Tasks;
using TopTwitchClipBotFunctions.Wrappers;
using TopTwitchClipBotModel;

namespace TopTwitchClipBotFunctions.Helpers
{
    public class PostChannelConfigHelper
    {
        readonly ILoggerWrapper _Log;
        readonly ITopTwitchClipBotContext _Context;
        public PostChannelConfigHelper(ILoggerWrapper log, ITopTwitchClipBotContext context)
        {
            _Log = log;
            _Context = context;
        }
        public async Task<ChannelConfigContainer> PostChannelConfigAsync(decimal channelId, ChannelConfigContainer container)
        {
            _Log.LogInformation($"Posting channel config for channel '{channelId}'.");
            var result = await _Context.SetChannelConfigAsync(channelId, container);
            _Log.LogInformation($"Posted channel config for channel '{channelId}'.");
            return result;
        }
    }
}
