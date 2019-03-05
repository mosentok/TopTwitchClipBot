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
        public int ChannelTopClipConfigId { get; set; }
        [Column(TypeName = "varchar(100)")]
        public string Slug { get; set; }
        [Column(TypeName = "varchar(255)")]
        public string ClipUrl { get; set; }
        public DateTime Stamp { get; set; }
        public virtual ChannelTopClipConfig ChannelTopClipConfig { get; set; }
    }
}
