using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Library.DataAccess.Domain
{
    public class Categories
    {
        [Key]
        [Display(Name = "ID")]

        public int CATEGORY_ID { get; set; }

        [Required(ErrorMessage = "Nombre es Obligatorio")]
        [StringLength(50, ErrorMessage = "Maximo 50 Caracteres")]
        [Display(Name = "Nombre")]

        public string CATEGORY_NAME { get; set; }
         [JsonIgnore]
        public List<Books> Books { get; set; } //propiedad de navegacion.


        [NotMapped]
        public int Top_Aux { get; set; }
    }
}
