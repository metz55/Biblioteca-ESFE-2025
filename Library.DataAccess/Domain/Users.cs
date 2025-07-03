using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.DataAccess.Domain
{
    public class Users
    {
        [Key]
        [Display(Name = "ID")]
        public long USER_ID { get; set; }

        [Required(ErrorMessage = "El rol de usuario es obligatorio")]
        [Display(Name = "Rol")]
        [ForeignKey("Users_Roles")]
        public long ROlE_ID { get; set; }

        [Required(ErrorMessage = "El usuario es obligatorio")]
        [Display(Name = "Usuario")]
        public string USER_NAME { get; set; }

        [Required(ErrorMessage = "La Contraseña es obligatoria")]
        [Display(Name = "Contraseña")]
        public string PASSWORD_HASH { get; set; }

        [Required(ErrorMessage = "Campo obligatorio")]
        [Display(Name = "Fecha de creación")]

        public DateTime CREATED_AT { get; set; }

        public Users_Roles Users_Roles { get; set; }

        [NotMapped]
        [Required(ErrorMessage = "Confirmar Contraseña")]
        [StringLength(120, ErrorMessage = "La clave debe estar entre 8 y 120 carácteres")]
        [DataType(DataType.Password)]
        [Compare("PASSWORD_HASH", ErrorMessage = "Contraseña y confirmar Contraseña" +
          " deben ser iguales")]
        [Display(Name = "Confirmar Contraseña")]
        public string ConfirmPassword { get; set; }

        [NotMapped]
        public int Top_Aux { get; set; }


    }
}
