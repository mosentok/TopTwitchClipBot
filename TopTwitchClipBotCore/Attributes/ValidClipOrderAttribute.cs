using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TopTwitchClipBotCore.Wrappers;

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
            var configWrapper = services.GetService<IConfigurationWrapper>();
            var clipOrders = configWrapper.Get<List<string>>("ClipOrders");
            var isValidClipOrder = clipOrders.Any(s => s.Equals(inputString, StringComparison.CurrentCultureIgnoreCase));
            if (isValidClipOrder)
                return Task.FromResult(PreconditionResult.FromSuccess());
            return Task.FromResult(PreconditionResult.FromError($"The input string '{inputString}' is not a valid clip order."));
        }
    }
}
