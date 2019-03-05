using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using TopTwitchClipBotFunctions.Helpers;
using TopTwitchClipBotFunctions.Wrappers;
using TopTwitchClipBotModel;

namespace TopTwitchClipBotFunctions.Functions
{
    public static class PostClipsFunction
    {
        [FunctionName(nameof(PostClipsFunction))]
        public static async Task Run([TimerTrigger("%PostClipsFunctionCron%")]TimerInfo myTimer, ILogger log)
        {
            var topClipsEndpoint = Environment.GetEnvironmentVariable("TwitchTopClipsEndpoint");
            var clientId = Environment.GetEnvironmentVariable("TwitchClientId");
            var accept = Environment.GetEnvironmentVariable("TwitchAcceptHeaderValue");
            var botToken = Environment.GetEnvironmentVariable("BotToken");
            var connectionString = Environment.GetEnvironmentVariable("TopTwitchClipBotConnectionString");
            var yesterday = DateTime.Now.AddDays(-1);
            var logWrapper = new LoggerWrapper(log);
            using (var twitchWrapper = new TwitchWrapper())
            using (var discordWrapper = new DiscordWrapper(botToken))
            using (var context = new TopTwitchClipBotContext(connectionString))
            {
                var helper = new PostClipsHelper(twitchWrapper, discordWrapper, context, logWrapper);
                await helper.PostClips(topClipsEndpoint, clientId, accept, yesterday);
            }
        }
    }
}
