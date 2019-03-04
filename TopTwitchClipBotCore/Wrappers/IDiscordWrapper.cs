using Discord;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace TopTwitchClipBotCore.Wrappers
{
    public interface IDiscordWrapper
    {
        IUser CurrentUser { get; }
        IDiscordClient DiscordClient { get; }
        Task LogInAsync();
        Task StartAsync();
        void AddMessageReceivedHandler(Func<SocketMessage, Task> handler);
        void AddLogHandler(Func<LogMessage, Task> handler);
    }
}