using Discord;
using Discord.Commands;
using System.Collections.Generic;
using TopTwitchClipBotCore.Models;
using TopTwitchClipBotCore.Wrappers;

namespace TopTwitchClipBotCore.Helpers
{
    public class BotHelper : IBotHelper
    {
        readonly IConfigurationWrapper _ConfigWrapper;
        public BotHelper(IConfigurationWrapper configWrapper)
        {
            _ConfigWrapper = configWrapper;
        }
        public ShouldProcessMessageContainer ShouldProcessMessage(IMessage message, IUser currentUser, Dictionary<decimal, string> prefixDictionary)
        {
            var isDmChannel = message.Channel is IDMChannel;
            if (isDmChannel)
                return new ShouldProcessMessageContainer(false);
            if (message.Author.IsBot || !(message is IUserMessage userMessage))
                return new ShouldProcessMessageContainer(false);
            var found = prefixDictionary.TryGetValue(message.Channel.Id, out var prefix);
            if (!found)
                prefix = _ConfigWrapper["DefaultPrefix"];
            var argPos = 0;
            if (!userMessage.HasStringPrefix(prefix, ref argPos) && !userMessage.HasMentionPrefix(currentUser, ref argPos))
                return new ShouldProcessMessageContainer(false);
            return new ShouldProcessMessageContainer(true, userMessage, argPos);
        }
    }
}
