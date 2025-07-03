using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.DataAccess.Domain;

public class PostsDocs
{
    [Key]
    public long Id { get; set; }

    [Required]
    [ForeignKey("Posts")]
    [Column("POST_ID")]
    public long PostId { get; set; }

    [Required]
    [MaxLength(300)]
    public string Path { get; set; } = string.Empty;
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;

    public Posts Post { get; set; }
        


}