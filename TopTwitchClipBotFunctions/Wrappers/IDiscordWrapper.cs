using System.Threading.Tasks;
using Discord;

namespace TopTwitchClipBotFunctions.Wrappers
{
    public interface IDiscordWrapper
    {
        Task<IMessageChannel> GetChannelAsync(decimal id);
        Task LogInAsync();
        Task LogOutAsync();
    }
}