using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using TopTwitchClipBotFunctions.Helpers;
using TopTwitchClipBotFunctions.Wrappers;
using TopTwitchClipBotModel;

namespace TopTwitchClipBotFunctions.Functions
{
    public static class GetBroadcasterConfigFunction
    {
        [FunctionName(nameof(GetBroadcasterConfigFunction))]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "channels/{channelid:decimal}/broadcasters/{broadcaster}")] HttpRequest req, decimal channelId, string broadcaster, ILogger log)
        {
            var connectionString = Environment.GetEnvironmentVariable("TopTwitchClipBotConnectionString");
            var logWrapper = new LoggerWrapper(log);
            BroadcasterConfigContainer result;
            using (var context = new TopTwitchClipBotContext(connectionString))
            {
                var helper = new GetBroadcasterConfigHelper(logWrapper, context);
                result = await helper.GetBroadcasterConfigAsync(channelId, broadcaster);
            }
            return new OkObjectResult(result);
        }
    }
}
