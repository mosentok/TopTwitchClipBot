namespace TopTwitchClipBotCore.Wrappers
{
    public interface IConfigurationWrapper
    {
        string this[string key] { get; }

        T Get<T>(string sectionName) where T : class;
        T GetValue<T>(string key);
    }
}