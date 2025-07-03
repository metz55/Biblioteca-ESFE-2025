using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Library.DataAccess.Domain;

public class PostsCategories
{
    [Key]
    public long Id { get; set; }
    public string Name { get; set; }
}