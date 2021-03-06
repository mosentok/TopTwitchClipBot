﻿using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TopTwitchClipBotModel
{
    public class ChannelConfig
    {
        [Key]
        [Column(TypeName = "decimal(20,0)")]
        public decimal ChannelId { get; set; }
        [Column(TypeName = "varchar(4)")]
        public string Prefix { get; set; }
        public int? MinPostingHour { get; set; }
        public int? MaxPostingHour { get; set; }
        public int? NumberOfClipsAtATime { get; set; }
        public long? TimeSpanBetweenClipsAsTicks { get; set; }
        public int? GlobalMinViews { get; set; }
        [Column(TypeName = "decimal(3,1)")]
        public decimal? UtcHourOffset { get; set; }
        public string ClipOrder { get; set; }
        public virtual ICollection<BroadcasterConfig> BroadcasterConfigs { get; set; }
    }
}
