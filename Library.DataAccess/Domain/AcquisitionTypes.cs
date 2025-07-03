using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.DataAccess.Domain
{
    public class AcquisitionTypes
    {
        [Key]
        [Display(Name ="ID")]
        public int ACQUISITION_ID { get; set; }

        [Required(ErrorMessage = "El Tipo de adquisición es Obligatorio")]
        [StringLength(50, ErrorMessage = "Maximo 50 Caracteres")]
        [Display(Name = "Nombre")]
        public string ACQUISITION_TYPE { get; set; }
        public List<Books> Books { get; set; } //propiedad de navegacion.

        [NotMapped]
        public int Top_Aux { get; set; }

    }
}
