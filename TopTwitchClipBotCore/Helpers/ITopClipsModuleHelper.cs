using Discord;
using Discord.Commands;
using TopTwitchClipBotCore.Enums;
using TopTwitchClipBotModel;

namespace TopTwitchClipBotCore.Helpers
{
    public interface ITopClipsModuleHelper
    {
        bool IsCorrectBetweenLength(string input, out string[] split);
        bool IsInRange(string postingHourString, out int postingHour);
        bool ShouldDeleteAll(string broadcaster);
        bool ShouldTurnCommandOff(string input);
        string DeterminePostWhen(ChannelConfigContainer container);
        string BuildStreamersText(ChannelConfigContainer container);
        string DetermineClipsAtATime(ChannelConfigContainer container);
        long? TicksFromIntervalTime(int interval, Time time);
        string TimeSpanBetweenClipsAsString(ChannelConfigContainer result);
        Embed BuildChannelConfigEmbed(ICommandContext context, string postWhen, string streamersText, string clipsAtATime, string timeSpanString);
    }
}