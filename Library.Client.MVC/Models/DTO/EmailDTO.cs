using System;

namespace Library.Client.MVC.Models.DTO;

public class EmailDTO
{
    public string Message {get;set;} = string.Empty;
    public string Subject {get;set;} = string.Empty;
    public string Subtitle {get;set;} = string.Empty;
    public string ReceptorName {get;set;} = string.Empty;
    public string ReceptorEmail {get;set;} = string.Empty;
    public string EmailBody {get;set;} = string.Empty;
    public bool IsLoanReminder {get;set;} = true;
}
