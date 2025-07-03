using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.DataAccess.Domain;

public class PostsImages
{
        [Key]
        public long ID { get; set; }

        [Required]
        [ForeignKey("Posts")]
        [Column("POST_ID")]
        public long POSTID { get; set; }
        public Posts Post { get; set; }

        [Required]
        [MaxLength(300)]
        public string PATH { get; set; } = string.Empty;

        
        


}