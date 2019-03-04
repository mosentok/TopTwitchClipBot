using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TopTwitchClipBotModel
{
    public class TopClipHistory
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        public decimal ChannelId { get; set; }
        public string Slug { get; set; }
        public string ClipUrl { get; set; }
        public DateTime Stamp { get; set; }
        public virtual ChannelTopClipConfig ChannelTopClipConfig { get; set; }
    }
}
