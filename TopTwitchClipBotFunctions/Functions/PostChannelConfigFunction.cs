using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using TopTwitchClipBotModel;
using TopTwitchClipBotFunctions.Extensions;
using TopTwitchClipBotFunctions.Helpers;
using TopTwitchClipBotFunctions.Wrappers;

namespace TopTwitchClipBotFunctions.Functions
{
    public static class PostChannelConfigFunction
    {
        [FunctionName(nameof(PostChannelConfigFunction))]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "channels/{channelid:decimal}")] HttpRequest req, decimal channelId, ILogger log)
        {
            var container = await req.Body.ReadToEndAsync<ChannelConfigContainer>();
            var connectionString = Environment.GetEnvironmentVariable("TopTwitchClipBotConnectionString");
            var logWrapper = new LoggerWrapper(log);
            ChannelConfigContainer result;
            using (var context = new TopTwitchClipBotContext(connectionString))
            {
                var helper = new PostChannelConfigHelper(logWrapper, context);
                result = await context.SetChannelConfigAsync(channelId, container);
            }
            return new OkObjectResult(result);
        }
    }
}
