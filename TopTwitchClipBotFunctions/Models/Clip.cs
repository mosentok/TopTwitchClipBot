using Newtonsoft.Json;
using System;

namespace TopTwitchClipBotFunctions.Models
{
    public class Clip
    {
        public string Slug { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
        public int Views { get; set; }
        public float Duration { get; set; }
        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }
    }
}
