using Discord.Commands;
using System;
using System.Threading.Tasks;

namespace TopTwitchClipBotCore.Attributes
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class OffCommandAttribute : ParameterPreconditionAttribute
    {
        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, ParameterInfo parameter, object value, IServiceProvider services)
        {
            var inputString = Convert.ToString(value);
            var isOffCommand = !string.IsNullOrEmpty(inputString) && inputString.Equals("off", StringComparison.InvariantCultureIgnoreCase);
            if (isOffCommand)
                return Task.FromResult(PreconditionResult.FromSuccess());
            return Task.FromResult(PreconditionResult.FromError($"The input string '{inputString}' is not equal to 'off'."));
        }
    }
}
