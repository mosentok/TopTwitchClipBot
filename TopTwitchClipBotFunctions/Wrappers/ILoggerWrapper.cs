using System;

namespace TopTwitchClipBotFunctions.Wrappers
{
    public interface ILoggerWrapper
    {
        void LogError(Exception exception, string message);
        void LogError(string message);
        void LogInformation(string message);
    }
}