using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TopTwitchClipBotModel
{
    public class BroadcasterConfig
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        [Column(TypeName = "decimal(20,0)")]
        public decimal ChannelId { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string Broadcaster { get; set; }
        public int? NumberOfClipsPerDay { get; set; }
        public virtual ChannelConfig ChannelConfig { get; set; }
        public virtual ICollection<BroadcasterHistory> BroadcasterHistories { get; set; }
    }
}
