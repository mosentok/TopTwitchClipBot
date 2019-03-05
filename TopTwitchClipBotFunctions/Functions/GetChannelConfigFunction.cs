using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using TopTwitchClipBotModel;
using TopTwitchClipBotFunctions.Helpers;
using TopTwitchClipBotFunctions.Wrappers;

namespace TopTwitchClipBotFunctions.Functions
{
    public static class GetChannelConfigFunction
    {
        [FunctionName(nameof(GetChannelConfigFunction))]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "channels/{channelid:decimal}")] HttpRequest req, decimal channelId, ILogger log)
        {
            var connectionString = Environment.GetEnvironmentVariable("TopTwitchClipBotConnectionString");
            var logWrapper = new LoggerWrapper(log);
            ChannelConfigContainer result;
            using (var context = new TopTwitchClipBotContext(connectionString))
            {
                var helper = new GetChannelConfigHelper(logWrapper, context);
                result = await helper.RunAsync(channelId);
            }
            return new OkObjectResult(result);
        }
    }
}
