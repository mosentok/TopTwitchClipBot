using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using TopTwitchClipBotFunctions.Models;
using TopTwitchClipBotFunctions.Extensions;
using System;
using TopTwitchClipBotModel;

namespace TopTwitchClipBotFunctions.Functions
{
    public static class PostChannelTopClipConfigFunction
    {
        [FunctionName(nameof(PostChannelTopClipConfigFunction))]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "channels/{channelid:decimal}/topclips/{broadcaster:string}")] HttpRequest req, decimal channelId, string broadcaster, ILogger log)
        {
            var container = await req.Body.ReadToEndAsync<ChannelTopClipConfigContainer>();
            var connectionString = Environment.GetEnvironmentVariable("TopTwitchClipBotConnectionString");
            ChannelTopClipConfigContainer result;
            using (var context = new TopTwitchClipBotContext(connectionString))
                result = await context.SetChannelTopClipConfigAsync(channelId, broadcaster, container);
            return new OkObjectResult(result);
        }
    }
}
