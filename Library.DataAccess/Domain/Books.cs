using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.DataAccess.Domain
{
    public class Books
    {
        [Key]
        [Display(Name = "ID")]

        public long BOOK_ID { get; set; }

        [Display(Name ="Categoría")]
        [ForeignKey("Categories")]
        [Required(ErrorMessage = "La Categoría es Obligatorio")]
        public int ID_CATEGORY { get; set; }
        public Categories Categories { get; set; }


        [Display(Name = "Adquisición")]
        [ForeignKey("AcquisitionTypes")]
        [Required(ErrorMessage = "El Tipo de Adquisición es Obligatorio")]
        public int ID_ACQUISITION { get; set; }
        public AcquisitionTypes AcquisitionTypes { get; set; }


        [Display(Name = "Editoriales")]
        [ForeignKey("Editorials")]
        [Required(ErrorMessage = "El Editorial es Obligatorio")]
        public long ID_EDITORIAL { get; set; }
        public Editorials Editorials { get; set; }


        [Display(Name = "Autor")]
        [ForeignKey("Authors")]
        [Required(ErrorMessage = "El Nombre del Autor es Obligatorio")]
        public long ID_AUTHOR { get; set; }
        public Authors Authors { get; set; }


        [Display(Name = "Edición")]
        [ForeignKey("Editions")]
        [Required(ErrorMessage = "El Tipo de Edición es Obligatorio")]
        public int ID_EDITION { get; set; }
        public Editions Editions { get; set; }


        [Display(Name = "Ciudad")]
        [ForeignKey("Countries")]
        [Required(ErrorMessage = "La Cuidad de Editorial es Obligatorio")]
        public int ID_COUNTRY { get; set; }
        public Countries Countries { get; set; }


        [Display(Name = "Catalogo")]
        [ForeignKey("Catalogs")]
        [Required(ErrorMessage = "El Catalogo del Libro es Obligatorio")]
        public int ID_CATALOG { get; set; }
        public Catalogs Catalogs { get; set; }

        [Display(Name = "Dewey")]
        [Required(ErrorMessage = "Dewey es Obligatorio")]
        [StringLength(50, ErrorMessage = "Maximo 50 Caracteres")]
        public string DEWEY { get; set; }

        [Display(Name = "Cuter")]
        [Required(ErrorMessage = "Cuter es Obligatorio")]
        [StringLength(50, ErrorMessage = "Maximo 50 Caracteres")]
        public string CUTER { get; set; }

        [Display(Name = "Título")]
        [Required(ErrorMessage = "El titulo es Obligatorio")]
        [StringLength(200, ErrorMessage = "Maximo 200 Caracteres")]
        public string TITLE { get; set; }

        [Display(Name = "Año")]
        [Required(ErrorMessage = "El Año es Obligatorio")]
        [StringLength(5, ErrorMessage = "Maximo 5 Caracteres")]
        public string YEAR { get; set; }

        [Display(Name = "Ejemplares")]
        [Required(ErrorMessage = "El Número de ejemplares Obligatorio")]
        public int EJEMPLARS { get; set; }


        [Display(Name = "Existencias")]
        public int EXISTENCES { get; set; }

        [NotMapped]
        [ValidateNever]
        public string CoverImagePath { get; set; }

        
        [Display(Name = "Portada")]
        public string COVER { get; set; }

        [NotMapped]
        public int Top_Aux { get; set; }
    }
    public class Books2
    {
        public long BOOK_ID { get; set; }
        public int EXISTENCES { get; set; }

    }
}
