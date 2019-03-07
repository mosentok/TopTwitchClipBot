using System.Linq;
using System.Threading.Tasks;
using TopTwitchClipBotFunctions.Models;
using TopTwitchClipBotFunctions.Wrappers;
using TopTwitchClipBotModel;

namespace TopTwitchClipBotFunctions.Helpers
{
    public class PostBroadcasterConfigHelper
    {
        readonly ILoggerWrapper _Log;
        readonly ITopTwitchClipBotContext _Context;
        readonly ITwitchWrapper _TwitchWrapper;
        public PostBroadcasterConfigHelper(ILoggerWrapper log, ITopTwitchClipBotContext context, ITwitchWrapper twitchWrapper)
        {
            _Log = log;
            _Context = context;
            _TwitchWrapper = twitchWrapper;
        }
        public async Task<PostBroadcasterConfigResponse> PostBroadcasterConfigAsync(string endpoint, string clientId, string accept, decimal channelId, string broadcaster, BroadcasterConfigContainer container)
        {
            _Log.LogInformation($"Checking that broadcaster '{broadcaster}' exists.");
            var userResponse = await _TwitchWrapper.GetUsers(endpoint, clientId, accept, broadcaster);
            if (!userResponse.Users.Any())
            {
                var errorMessage = $"Could not find streamer '{broadcaster}'.";
                _Log.LogInformation(errorMessage);
                return new PostBroadcasterConfigResponse(errorMessage);
            }
            var displayName = userResponse.Users[0].DisplayName;
            _Log.LogInformation($"Found broadcaster '{broadcaster}' with display name '{displayName}'.");
            _Log.LogInformation($"Posting broadcaster config for channel '{channelId}' broadcaster '{displayName}'.");
            var result = await _Context.SetBroadcasterConfigAsync(channelId, displayName, container);
            _Log.LogInformation($"Posted broadcaster config for channel '{channelId}' broadcaster '{displayName}'.");
            return new PostBroadcasterConfigResponse(result);
        }
    }
}
