using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.DataAccess.Domain
{
    public class Users_Roles
    {
        [Key]
        [Display(Name = "ID")]
        public long USER_ROLE_ID { get; set; }

        [Required(ErrorMessage = "Este Campo es Obligatorio")]
        [Display(Name = "Nombre")]
        public string ROLE { get; set; }

        [NotMapped]
        public int Top_Aux { get; set; }
    }
}
