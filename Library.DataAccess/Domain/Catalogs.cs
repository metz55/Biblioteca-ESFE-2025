using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.DataAccess.Domain
{
    public class Catalogs
    {
        [Key]
        [Display(Name = "ID")]

        public int CATALOG_ID { get; set; }

        [Required(ErrorMessage = "Nombre es Obligatorio")]
        [StringLength(100, ErrorMessage = "Maximo 100 Caracteres")]
        [Display(Name = "Nombre")]

        public string CATALOG_NAME { get; set; }
        public List<Books> Books { get; set; } //propiedad de navegacion.


        [NotMapped]
        public int Top_Aux { get; set; }
    }
}
