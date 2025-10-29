using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.DataAccess.Domain
{
    public class Book
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = "";
        public int Año { get; set; }
        public int Ejemplares { get; set; }
        public int Existencias { get; set; }
        public int? CategoriaId { get; set; }
        public int? AutorId { get; set; }
        public int? EditorialId { get; set; }
        public int? PaisId { get; set; }
        public int? EdicionId { get; set; }
        public int? TipoAdquisicionId { get; set; }
        public int? CatalogoId { get; set; }
        public string? CoverPath { get; set; }
        public DateTime FechaCreacion { get; set; }
    }

}
