using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using TopTwitchClipBotModel;

namespace TopTwitchClipBotFunctions.Functions
{
    public static class GetChannelConfigFunction
    {
        [FunctionName(nameof(GetChannelConfigFunction))]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "channels/{channelid:decimal}")] HttpRequest req, decimal channelId, ILogger log)
        {
            log.LogInformation($"Getting channel config for channel '{channelId}'.");
            var connectionString = Environment.GetEnvironmentVariable("TopTwitchClipBotConnectionString");
            ChannelConfigContainer result;
            using (var context = new TopTwitchClipBotContext(connectionString))
                result = await context.GetChannelConfigAsync(channelId);
            log.LogInformation($"Got channel config for channel '{channelId}'.");
            return new OkObjectResult(result);
        }
    }
}
