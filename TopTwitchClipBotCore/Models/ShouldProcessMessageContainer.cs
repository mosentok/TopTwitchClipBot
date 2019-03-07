using Discord;

namespace TopTwitchClipBotCore.Models
{
    public class ShouldProcessMessageContainer
    {
        public bool ShouldProcessMessage { get; set; }
        public IUserMessage UserMessage { get; set; }
        public int ArgPos { get; set; }
        public ShouldProcessMessageContainer(bool shouldProcessMessage)
        {
            ShouldProcessMessage = shouldProcessMessage;
        }
        public ShouldProcessMessageContainer(bool shouldProcessMessage, IUserMessage userMessage, int argPos)
        {
            ShouldProcessMessage = shouldProcessMessage;
            UserMessage = userMessage;
            ArgPos = argPos;
        }
    }
}
