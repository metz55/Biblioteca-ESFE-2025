using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Library.DataAccess.Domain
{
    public class LoanTypes
    {
        [Key]
        [Display(Name = "ID")]

        public int TYPES_ID { get; set; }

        [Required(ErrorMessage = "Nombre es Obligatorio")]
        [StringLength(50, ErrorMessage = "Maximo 50 Caracteres")]
        [Display(Name = "Nombre")]

        public string TYPES_NAME { get; set; }

        [NotMapped]
        public int Top_Aux { get; set; }
    }
}
