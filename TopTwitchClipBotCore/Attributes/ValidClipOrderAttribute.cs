using Discord.Commands;
using System;
using System.Threading.Tasks;

namespace TopTwitchClipBotCore.Attributes
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class ValidClipOrderAttribute : ParameterPreconditionAttribute
    {
        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, ParameterInfo parameter, object value, IServiceProvider services)
        {
            var inputString = Convert.ToString(value);
            if (string.IsNullOrEmpty(inputString))
                return Task.FromResult(PreconditionResult.FromError("Clip order must be provided."));
            switch (inputString.ToLower())
            {
                //TODO replace with config
                case "views":
                case "view count":
                case "oldest first":
                case "oldest":
                case "even mix":
                    return Task.FromResult(PreconditionResult.FromSuccess());
                default:
                    return Task.FromResult(PreconditionResult.FromError($"The input string '{inputString}' is not a valid clip order."));
            }
        }
    }
}
