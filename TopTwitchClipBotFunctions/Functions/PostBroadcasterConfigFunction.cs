using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using TopTwitchClipBotFunctions.Extensions;
using System;
using TopTwitchClipBotModel;

namespace TopTwitchClipBotFunctions.Functions
{
    public static class PostBroadcasterConfigFunction
    {
        [FunctionName(nameof(PostBroadcasterConfigFunction))]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "channels/{channelid:decimal}/broadcasters/{broadcaster}")] HttpRequest req, decimal channelId, string broadcaster, ILogger log)
        {
            var container = await req.Body.ReadToEndAsync<BroadcasterConfigContainer>();
            var connectionString = Environment.GetEnvironmentVariable("TopTwitchClipBotConnectionString");
            BroadcasterConfigContainer result;
            using (var context = new TopTwitchClipBotContext(connectionString))
                result = await context.SetBroadcasterConfigAsync(channelId, broadcaster, container);
            return new OkObjectResult(result);
        }
    }
}
