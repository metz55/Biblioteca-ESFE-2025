using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.DataAccess.Domain
{
    public class Editions
    {
        [Key]
        [Display(Name = "ID")]

        public int EDITION_ID { get; set; }

        [Required(ErrorMessage = "Nombre es Obligatorio")]
        [StringLength(50, ErrorMessage = "Maximo 50 Caracteres")]
        [Display(Name = "Número de Edición")]

        public string EDITION_NUMBER { get; set; }
        public List<Books> Books { get; set; } //propiedad de navegacion.


        [NotMapped]
        public int Top_Aux { get; set; }
    }
}
