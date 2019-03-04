using System.Threading.Tasks;
using TopTwitchClipBotFunctions.Models;

namespace TopTwitchClipBotFunctions.Wrappers
{
    public interface ITwitchWrapper
    {
        Task<GetClipsResponse> GetClips(string endpoint, string clientId, string accept);
    }
}