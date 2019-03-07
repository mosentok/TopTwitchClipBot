using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using TopTwitchClipBotModel;
using TopTwitchClipBotFunctions.Helpers;
using TopTwitchClipBotFunctions.Wrappers;

namespace TopTwitchClipBotFunctions.Functions
{
    public static class DeleteBroadcasterConfigFunction
    {
        [FunctionName(nameof(DeleteBroadcasterConfigFunction))]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "delete", Route = "channels/{channelid:decimal}/broadcasters/{broadcaster?}")] HttpRequest req, decimal channelId, string broadcaster, ILogger log)
        {
            var connectionString = Environment.GetEnvironmentVariable("TopTwitchClipBotConnectionString");
            var logWrapper = new LoggerWrapper(log);
            ChannelConfigContainer result;
            using (var context = new TopTwitchClipBotContext(connectionString))
            {
                var helper = new DeleteBroadcasterConfigHelper(logWrapper, context);
                result = await helper.DeleteBroadcasterConfigAsync(channelId, broadcaster);
            }
            return new OkObjectResult(result);
        }
    }
}
