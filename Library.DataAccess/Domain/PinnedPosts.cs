using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Library.DataAccess.Domain;

public class PinnedPosts
{
    [Key]
    public long ID { get; set; }

    [ForeignKey("Posts")]
    [Column("POST_ID")]
    public long POSTID { get; set; }
    public Posts POST { get; set; }
    public DateTime CREATED_AT {get; set;} = DateTime.Now;
    
}
