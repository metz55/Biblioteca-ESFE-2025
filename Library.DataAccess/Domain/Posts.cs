using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Library.DataAccess.Domain;

public class Posts
{
    [Key]
    [Display(Name ="ID")]
    public long ID {get; set;} 
    
    [ForeignKey("PostsCategories")]
    [Column("CATEGORY_ID")]
    [Required(ErrorMessage = "La categoría es obligatoria")]
    public long CATEGORYID { get; set; }

    [Required(ErrorMessage = "El Título es obligatorio")]
    [StringLength(250, ErrorMessage = "Máximo 250 Caracteres")]
    [Display(Name = "Títutlo")]
    public string TITLE { get; set; }

    [Required(ErrorMessage = "El Contenido es obligatorio")]
    [StringLength(4000, ErrorMessage = "Máximo 4000 Caracteres")]
    [Display(Name = "Contenido")]
    public string CONTENT { get; set; }

    [Required]
    public DateTime CREATED_AT { get; set; } = DateTime.Now;

    public ICollection<PostsImages> IMAGES { get; set; }
    public ICollection<PostsDocs> DOCS { get; set; }
    public PostsCategories CATEGORY { get; set; }

    [NotMapped]
    [ValidateNever]
    public ICollection<string> IMAGES_PATH { get; set; }

    [NotMapped]
    [ValidateNever]
    public ICollection<string> DOCS_PATH { get; set; }

    [NotMapped]
    [ValidateNever]
    public bool IS_PINNED { get; set; } = false;


}