using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using TopTwitchClipBotCore.Helpers;

namespace TopTwitchClipBotCore.Attributes
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class ValidUtcOffsetAttribute : ParameterPreconditionAttribute
    {
        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, ParameterInfo parameter, object value, IServiceProvider services)
        {
            var utcHourOffset = Convert.ToDecimal(value);
            var helper = services.GetRequiredService<ITopClipsModuleHelper>();
            var isInRange = helper.IsInUtcRange(utcHourOffset);
            if (!isInRange)
                return Task.FromResult(PreconditionResult.FromError("The time zone is invalid because it is not in range."));
            var isValidFraction = helper.IsValidTimeZoneFraction(utcHourOffset);
            if (!isValidFraction)
                return Task.FromResult(PreconditionResult.FromError("The time zone is invalid because the fraction is invalid."));
            return Task.FromResult(PreconditionResult.FromSuccess());
        }
    }
}
