using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.DataAccess.Domain
{
    public class Authors
    {
        [Key]
        [Display(Name = "ID")]
        public long AUTHOR_ID { get; set; }

        [Required(ErrorMessage = "Nombre es Obligatorio")]
        [StringLength(200, ErrorMessage = "Maximo 200 Caracteres")]
        [Display(Name = "Nombre")]

        public string AUTHOR_NAME { get; set; }
        public List<Books> Books { get; set; } //propiedad de navegacion.


        [NotMapped]
        public int Top_Aux { get; set; }
    }
}
