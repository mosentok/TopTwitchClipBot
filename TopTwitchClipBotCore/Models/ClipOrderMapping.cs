using System.Collections.Generic;

namespace TopTwitchClipBotCore.Models
{
    public class ClipOrderMapping
    {
        public bool IsDefault { get; set; }
        public string Description { get; set; }
        public List<string> MapsToClipOrders { get; set; }
    }
}
