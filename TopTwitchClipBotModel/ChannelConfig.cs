using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TopTwitchClipBotModel
{
    public class ChannelConfig
    {
        [Key]
        public decimal ChannelId { get; set; }
        public string Prefix { get; set; }
        public int? MinPostingHour { get; set; }
        public int? MaxPostingHour { get; set; }
        public virtual ICollection<ChannelTopClipConfig> ChannelTopClipConfigs { get; set; }
    }
}
