using Discord;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace TopTwitchClipBotCore.Wrappers
{
    public class DiscordWrapper : IDiscordWrapper, IDisposable
    {
        readonly TaskCompletionSource<bool> _LoggedInSource = new TaskCompletionSource<bool>();
        readonly TaskCompletionSource<bool> _ReadySource = new TaskCompletionSource<bool>();
        readonly TaskCompletionSource<bool> _LoggedOutSource = new TaskCompletionSource<bool>();
        readonly DiscordSocketClient _DiscordClient = new DiscordSocketClient();
        readonly string _BotToken;
        public IUser CurrentUser => _DiscordClient.CurrentUser;
        public IDiscordClient DiscordClient => _DiscordClient;
        public DiscordWrapper(string botToken)
        {
            _BotToken = botToken;
        }
        public async Task LogInAsync()
        {
            _DiscordClient.LoggedIn += LoggedIn;
            await _DiscordClient.LoginAsync(TokenType.Bot, _BotToken);
            await _LoggedInSource.Task;
            _DiscordClient.LoggedIn -= LoggedIn;
        }
        Task LoggedIn()
        {
            _LoggedInSource.SetResult(true);
            return Task.CompletedTask;
        }
        public async Task StartAsync()
        {
            _DiscordClient.Ready += Ready;
            await _DiscordClient.StartAsync();
            await _ReadySource.Task;
            _DiscordClient.Ready -= Ready;
        }
        Task Ready()
        {
            _ReadySource.SetResult(true);
            return Task.CompletedTask;
        }
        public void AddMessageReceivedHandler(Func<SocketMessage, Task> handler)
        {
            _DiscordClient.MessageReceived += handler;
        }
        public void AddLogHandler(Func<LogMessage, Task> handler)
        {
            _DiscordClient.Log += handler;
        }
        public void Dispose()
        {
            LogOutAsync().Wait();
            _DiscordClient?.Dispose();
        }
        async Task LogOutAsync()
        {
            _DiscordClient.LoggedOut += LoggedOut;
            await _DiscordClient.LogoutAsync();
            await _LoggedOutSource.Task;
            _DiscordClient.LoggedOut -= LoggedOut;
        }
        Task LoggedOut()
        {
            _LoggedOutSource.SetResult(true);
            return Task.CompletedTask;
        }
    }
}
