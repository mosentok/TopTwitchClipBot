using Newtonsoft.Json;

namespace TopTwitchClipBotFunctions.Models
{
    public class User
    {
        [JsonProperty(PropertyName = "display_name")]
        public string DisplayName { get; set; }
    }

}
