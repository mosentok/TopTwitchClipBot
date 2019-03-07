using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading.Tasks;
using TopTwitchClipBotCore.Helpers;
using TopTwitchClipBotCore.Wrappers;

namespace TopTwitchClipBotCore
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();
            var configWrapper = new ConfigurationWrapper(config);
            var topClipsModuleHelper = new TopClipsModuleHelper(configWrapper);
            var botHelper = new BotHelper(configWrapper);
            using (var functionWrapper = new FunctionWrapper(configWrapper))
            using (var discordWrapper = new DiscordWrapper(configWrapper["BotToken"]))
            using (var services = new ServiceCollection()
                .AddSingleton<IDiscordWrapper>(discordWrapper)
                .AddSingleton<IConfigurationWrapper>(configWrapper)
                .AddSingleton<IFunctionWrapper>(functionWrapper)
                .AddSingleton<ITopClipsModuleHelper>(topClipsModuleHelper)
                .AddSingleton<IBotHelper>(botHelper)
                .AddLogging(s => s.AddConsole())
                .BuildServiceProvider())
            {
                var bot = new Bot(services);
                await bot.RunAsync();
                await Task.Delay(-1);
            }
        }
    }
}
