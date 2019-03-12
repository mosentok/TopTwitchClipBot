using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TopTwitchClipBotCore.Helpers;
using TopTwitchClipBotCore.Wrappers;

namespace TopTwitchClipBotCore
{
    public class Bot
    {
        readonly IServiceProvider _Services;
        readonly IDiscordWrapper _DiscordWrapper;
        readonly IConfigurationWrapper _ConfigWrapper;
        readonly ILogger _Log;
        readonly IBotHelper _BotHelper;
        readonly CommandService _Commands;
        List<string> _ModuleNames;
        Dictionary<decimal, string> _PrefixDictionary;
        public Bot(IServiceProvider services)
        {
            _Services = services;
            _DiscordWrapper = _Services.GetService<IDiscordWrapper>();
            _ConfigWrapper = _Services.GetService<IConfigurationWrapper>();
            _Log = _Services.GetService<ILogger<Bot>>();
            _BotHelper = _Services.GetService<IBotHelper>();
            _Commands = new CommandService();
        }
        public async Task RunAsync()
        {
            _DiscordWrapper.AddLogHandler(LogReceived);
            await _DiscordWrapper.LogInAsync();
            await _DiscordWrapper.StartAsync();
            await _Commands.AddModulesAsync(Assembly.GetEntryAssembly(), _Services);
            _ModuleNames = DetermineModuleNames();
            _PrefixDictionary = new Dictionary<decimal, string>(); //TODO
            _DiscordWrapper.AddMessageReceivedHandler(MessageReceived);
            var playingGame = _ConfigWrapper["PlayingGame"];
            await _DiscordWrapper.SetGameAsync(playingGame);
        }
        List<string> DetermineModuleNames()
        {
            var moduleNames = _Commands.Modules.Select(s => s.Name);
            var aliases = _Commands.Modules.SelectMany(s => s.Aliases);
            return moduleNames.Concat(aliases).Distinct().Where(s => !string.IsNullOrEmpty(s)).ToList();
        }
        async Task MessageReceived(SocketMessage socketMessage)
        {
            var shouldProcess = _BotHelper.ShouldProcessMessage(socketMessage, _DiscordWrapper.CurrentUser, _PrefixDictionary);
            if (!shouldProcess.ShouldProcessMessage)
                return;
            var context = new CommandContext(_DiscordWrapper.DiscordClient, shouldProcess.UserMessage);
            var result = await _Commands.ExecuteAsync(context, shouldProcess.ArgPos, _Services);
            if (!result.IsSuccess &&
                result.Error.HasValue &&
                result.Error.Value != CommandError.UnknownCommand &&
                result.Error.Value != CommandError.BadArgCount &&
                result is ExecuteResult executeResult)
                _Log.LogError(executeResult.Exception, $"Error processing message content '{shouldProcess.UserMessage.Content}'.");
        }
        Task LogReceived(LogMessage logMessage)
        {
            switch (logMessage.Severity)
            {
                case LogSeverity.Critical:
                    _Log.LogCritical(logMessage.Message);
                    break;
                case LogSeverity.Error:
                    _Log.LogError(logMessage.Message);
                    break;
                case LogSeverity.Warning:
                    _Log.LogWarning(logMessage.Message);
                    break;
                case LogSeverity.Info:
                    _Log.LogInformation(logMessage.Message);
                    break;
                case LogSeverity.Verbose:
                    _Log.LogTrace(logMessage.Message);
                    break;
                case LogSeverity.Debug:
                    _Log.LogDebug(logMessage.Message);
                    break;
                default:
                    break;
            }
            return Task.CompletedTask;
        }
    }
}
