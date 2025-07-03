using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.DataAccess.Domain
{
    public class ReservationStatus
    {
        [Key]
        [Display(Name = "ID")]

        public int RESERVATION_ID { get; set; }

        [Required(ErrorMessage = "Nombre es Obligatorio")]
        [StringLength(50, ErrorMessage = "Maximo 50 Caracteres")]
        [Display(Name = "Nombre")]

        public string STATUS_NAME { get; set; }

        [NotMapped]
        public int Top_Aux { get; set; }
    }
}
