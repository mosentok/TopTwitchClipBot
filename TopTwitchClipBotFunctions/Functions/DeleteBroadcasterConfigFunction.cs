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
    public static class DeleteBroadcasterConfigFunction
    {
        [FunctionName(nameof(DeleteBroadcasterConfigFunction))]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "delete", Route = "channels/{channelid:decimal}/broadcasters/{broadcaster?}")] HttpRequest req, decimal channelId, string broadcaster, ILogger log)
        {
            log.LogInformation($"Deleting broadcaster config for channel '{channelId}' broadcaster '{broadcaster}'.");
            var connectionString = Environment.GetEnvironmentVariable("TopTwitchClipBotConnectionString");
            using (var context = new TopTwitchClipBotContext(connectionString))
                if (string.IsNullOrEmpty(broadcaster))
                    await context.DeleteBroadcasterConfigAsync(channelId);
                else
                    await context.DeleteBroadcasterConfigAsync(channelId, broadcaster);
            log.LogInformation($"Deleted broadcaster config for channel '{channelId}' broadcaster '{broadcaster}'.");
            return new NoContentResult();
        }
    }
}
