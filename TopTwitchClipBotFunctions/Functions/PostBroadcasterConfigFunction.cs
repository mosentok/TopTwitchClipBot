using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using TopTwitchClipBotFunctions.Extensions;
using System;
using TopTwitchClipBotModel;
using TopTwitchClipBotFunctions.Helpers;
using TopTwitchClipBotFunctions.Wrappers;
using TopTwitchClipBotFunctions.Models;

namespace TopTwitchClipBotFunctions.Functions
{
    public static class PostBroadcasterConfigFunction
    {
        [FunctionName(nameof(PostBroadcasterConfigFunction))]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "channels/{channelid:decimal}/broadcasters/{broadcaster}")] HttpRequest req, decimal channelId, string broadcaster, ILogger log)
        {
            var getUsersEndpoint = Environment.GetEnvironmentVariable("TwitchGetUsersEndpoint");
            var clientId = Environment.GetEnvironmentVariable("TwitchClientId");
            var accept = Environment.GetEnvironmentVariable("TwitchAcceptHeaderValue");
            var connectionString = Environment.GetEnvironmentVariable("TopTwitchClipBotConnectionString");
            var container = await req.Body.ReadToEndAsync<BroadcasterConfigContainer>();
            var logWrapper = new LoggerWrapper(log);
            PostBroadcasterConfigResponse result;
            using (var twitchWrapper = new TwitchWrapper())
            using (var context = new TopTwitchClipBotContext(connectionString))
            {
                var helper = new PostBroadcasterConfigHelper(logWrapper, context, twitchWrapper);
                result = await helper.PostBroadcasterConfigAsync(getUsersEndpoint, clientId, accept, channelId, broadcaster, container);
            }
            return new OkObjectResult(result);
        }
    }
}
