﻿using Discord;
using Discord.Commands;
using TopTwitchClipBotModel;

namespace TopTwitchClipBotCore.Helpers
{
    public interface ITopClipsModuleHelper
    {
        bool IsCorrectBetweenLength(string input, out string[] split);
        bool IsInRange(string postingHourString, out int postingHour);
        bool ShouldDeleteAll(string broadcaster);
        bool ShouldTurnCommandOff(string input);
        Embed BuildChannelConfigEmbed(ChannelConfigContainer container, ICommandContext context);
    }
}