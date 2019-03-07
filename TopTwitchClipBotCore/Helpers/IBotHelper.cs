using System.Collections.Generic;
using Discord;
using TopTwitchClipBotCore.Models;

namespace TopTwitchClipBotCore.Helpers
{
    public interface IBotHelper
    {
        ShouldProcessMessageContainer ShouldProcessMessage(IMessage socketMessage, IUser currentUser, Dictionary<decimal, string> prefixDictionary);
    }
}