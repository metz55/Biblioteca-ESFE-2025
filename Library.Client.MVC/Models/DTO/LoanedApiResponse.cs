using System.ComponentModel.DataAnnotations;
using Library.DataAccess.Domain;

namespace Library.Client.MVC.Models.DTO;

public class LoanedApiResponse
{
    public long LoanId { get; set; }
    public long LenderId { get; set; }
    public string StudentCode { get; set; } = string.Empty;
    public LoanedBookApiResponse Book { get; set; }
    public string LoanType {get; set;}
    public string ReservationStatus {get; set;}
    public decimal Fee { get; set; } 
    public bool Status { get; set; }
    public LoanedDateApiResponse Date {get; set;}
    public DateTime RegistrationDate {get; set;}
    public int DaysToReturn {get; set; } //Dias que faltan para entregar el libro prestado
    public int DaysLate {get; set; } //Dias de retraso al entregar el libro prestado
    public bool IsExpired {get; set;}
}

public class LoanedBookApiResponse
{
    public long BookId {get; set;}
    public string Category {get; set;}
    public string Title {get; set;}
}

public class LoanedDateApiResponse
{
    public long LoanDateId {get; set;}
    public DateTime StartDate {get; set;}
    public DateTime EndDate {get; set;}

}
