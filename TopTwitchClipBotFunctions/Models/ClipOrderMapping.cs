using System.Collections.Generic;

namespace TopTwitchClipBotFunctions.Models
{
    public class ClipOrderMapping
    {
        public bool IsDefault { get; set; }
        public ClipOrderKind ClipOrderKind { get; set; }
        public List<string> MapsToClipOrders { get; set; }
    }
}
