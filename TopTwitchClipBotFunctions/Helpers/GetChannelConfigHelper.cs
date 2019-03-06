using System.Threading.Tasks;
using TopTwitchClipBotFunctions.Wrappers;
using TopTwitchClipBotModel;

namespace TopTwitchClipBotFunctions.Helpers
{
    public class GetChannelConfigHelper
    {
        readonly ILoggerWrapper _Log;
        readonly ITopTwitchClipBotContext _Context;
        public GetChannelConfigHelper(ILoggerWrapper log, ITopTwitchClipBotContext context)
        {
            _Log = log;
            _Context = context;
        }
        public async Task<ChannelConfigContainer> GetChannelConfigAsync(decimal channelId)
        {
            _Log.LogInformation($"Getting channel config for channel '{channelId}'.");
            var result = await _Context.GetChannelConfigAsync(channelId);
            _Log.LogInformation($"Got channel config for channel '{channelId}'.");
            return result;
        }
    }
}
