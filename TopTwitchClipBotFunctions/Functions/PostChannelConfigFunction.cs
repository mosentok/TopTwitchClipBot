using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using TopTwitchClipBotModel;
using TopTwitchClipBotFunctions.Extensions;

namespace TopTwitchClipBotFunctions.Functions
{
    public static class PostChannelConfigFunction
    {
        [FunctionName(nameof(PostChannelConfigFunction))]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "channels/{channelid:decimal}")] HttpRequest req, decimal channelId, ILogger log)
        {
            log.LogInformation($"Posting channel config for channel '{channelId}'.");
            var container = await req.Body.ReadToEndAsync<ChannelConfigContainer>();
            var connectionString = Environment.GetEnvironmentVariable("TopTwitchClipBotConnectionString");
            ChannelConfigContainer result;
            using (var context = new TopTwitchClipBotContext(connectionString))
                result = await context.SetChannelConfigAsync(channelId, container);
            log.LogInformation($"Posted channel config for channel '{channelId}'.");
            return new OkObjectResult(result);
        }
    }
}
