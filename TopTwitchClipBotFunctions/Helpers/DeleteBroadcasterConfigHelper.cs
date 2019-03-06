using System.Threading.Tasks;
using TopTwitchClipBotFunctions.Wrappers;
using TopTwitchClipBotModel;

namespace TopTwitchClipBotFunctions.Helpers
{
    public class DeleteBroadcasterConfigHelper
    {
        readonly ILoggerWrapper _Log;
        readonly ITopTwitchClipBotContext _Context;
        public DeleteBroadcasterConfigHelper(ILoggerWrapper log, ITopTwitchClipBotContext context)
        {
            _Log = log;
            _Context = context;
        }
        public async Task DeleteBroadcasterConfigAsync(decimal channelId, string broadcaster)
        {
            if (string.IsNullOrEmpty(broadcaster))
            {
                _Log.LogInformation($"Deleting broadcaster config for channel '{channelId}'.");
                await _Context.DeleteBroadcasterConfigAsync(channelId);
                _Log.LogInformation($"Deleted broadcaster config for channel '{channelId}'.");
            }
            else
            {
                _Log.LogInformation($"Deleting broadcaster config for channel '{channelId}' broadcaster '{broadcaster}'.");
                await _Context.DeleteBroadcasterConfigAsync(channelId, broadcaster);
                _Log.LogInformation($"Deleted broadcaster config for channel '{channelId}' broadcaster '{broadcaster}'.");
            }
        }
    }
}
