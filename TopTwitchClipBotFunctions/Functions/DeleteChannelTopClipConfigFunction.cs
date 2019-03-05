using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using TopTwitchClipBotModel;

namespace TopTwitchClipBotFunctions.Functions
{
    public static class DeleteChannelTopClipConfigFunction
    {
        [FunctionName(nameof(DeleteChannelTopClipConfigFunction))]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "delete", Route = "channels/{channelid:decimal}/topclips/{broadcaster?}")] HttpRequest req, decimal channelId, string broadcaster, ILogger log)
        {
            var connectionString = Environment.GetEnvironmentVariable("TopTwitchClipBotConnectionString");
            using (var context = new TopTwitchClipBotContext(connectionString))
                if (string.IsNullOrEmpty(broadcaster))
                    await context.DeleteChannelTopClipConfigAsync(channelId);
                else
                    await context.DeleteChannelTopClipConfigAsync(channelId, broadcaster);
            return new NoContentResult();
        }
    }
}
