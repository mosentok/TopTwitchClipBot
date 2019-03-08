using System.Threading.Tasks;
using TopTwitchClipBotFunctions.Models;

namespace TopTwitchClipBotFunctions.Wrappers
{
    public interface ITwitchWrapper
    {
        Task<GetUsersResponse> GetUsers(string endpoint, string clientId, string accept, string broadcaster);
        Task<GetClipsResponse> GetClips(string endpoint, string clientId, string accept);
    }
}