using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.DataAccess.Domain
{
    public class Loans
    {
        [Key]
        public long LOAN_ID { get; set; }
        [Display(Name = "Tipo de Prestamo")]
        [ForeignKey("LoanTypes")]
        [Required(ErrorMessage = "El tipo de prestamo es Obligatorio")]
        public int ID_TYPE { get; set; }
        public LoanTypes LoanTypes { get; set; }


        [Display(Name = "Estado de Reservación")]
        [ForeignKey("ReservationStatus")]
        [Required(ErrorMessage = "El ID de la reservación es Obligatorio")]
        public int ID_RESERVATION { get; set; }
        public ReservationStatus ReservationStatus { get; set; }


        [Display(Name = "Código del Estudiante")]
        public long ID_LENDER { get; set; }
        public long USER_ID { get; set; }


        [Display(Name = "Libro")]
        [ForeignKey("Books")]
        [Required(ErrorMessage = "El nombre del Libro es Obligatorio")]
        public long ID_BOOK { get; set; }
        public Books Books { get; set; }

        [Required(ErrorMessage = "El número del ejemplar es Obligatorio")]
        public int COPY { get; set; }
        
        public decimal FEE { get; set; }

        [Required(ErrorMessage = "El contacto es Obligatorio")]
        [StringLength(9, ErrorMessage = "Maximo 9 Caracteres")]
        public string LENDER_CONTACT { get; set; } = string.Empty;


        [Required(ErrorMessage = "La fecha de registro es Obligatorio")]
        public DateTime REGISTRATION_DATE { get; set; }

        [Display(Name = "ESTADO")]
        [Required(ErrorMessage = "El estatus es Obligatorio")]
        public bool STATUS { get; set; }

        [NotMapped]
        public int Top_Aux { get; set; }

        // Propiedad de navegación para la relación uno-a-muchos con LoanDates
        public ICollection<LoanDates> LoanDates { get; set; }

    }
    public class Loans2
    {
        public long LOAN_ID { get; set; }
        public bool STATUS { get; set; }

    }
    public enum Status_Loans
    {
        ACTIVO = 1,
        INACTIVO = 0
    }
}
