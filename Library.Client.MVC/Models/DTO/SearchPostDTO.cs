using Library.DataAccess.Domain;

namespace Library.Client.MVC.Models.DTO;

public class SearchPostDTO
{
    public string Query { get; set;} = string.Empty;
    public int CategoryId { get; set;}
    public DateTime Date {get; set;}
}