using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TopTwitchClipBotModel
{
    public class ChannelTopClipConfig
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        public decimal ChannelId { get; set; }
        public string Broadcaster { get; set; }
        public int? NumberOfClipsPerDay { get; set; }
        public virtual ChannelConfig ChannelConfig { get; set; }
        public virtual ICollection<TopClipHistory> TopClipHistories { get; set; }
    }
}
