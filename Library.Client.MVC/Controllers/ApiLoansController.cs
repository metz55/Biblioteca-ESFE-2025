using Library.BusinessRules;
using Library.Client.MVC.Models;
using Library.Client.MVC.Models.DTO;
using Library.Client.MVC.services;
using Library.DataAccess.Domain;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Library.Client.MVC.Controllers;

[ApiController]
[Route("api/loans")]
public class ApiLoansController : ControllerBase
{
    private readonly LoanService _loanService;
    
    public ApiLoansController(LoanService loanService)
    {
        _loanService = loanService;
    }

    [HttpGet("loaned/{code}")]
    public async Task<IActionResult> GetLoansByStudentCode(string code)
    {
        var (loans, loansDates) = await _loanService.GetStudentLoans(code);
        if(loans == null || loansDates == null || loans.Count() == 0 || loansDates.Count() == 0)
        {
            return NotFound(new {Loans=new{}, Success=false}); 
        }
        List<LoanedApiResponse> loansResponse = new List<LoanedApiResponse>();
        foreach(var loan in loans)
        {
            var days = (loansDates.FirstOrDefault(x=> x.ID_LOAN == loan.LOAN_ID).END_DATE - DateTime.Now).Days;
            var days2return = days + 1;
            var daysLate = Math.Abs(days);

            LoanedApiResponse loanedApiResponse = new LoanedApiResponse()
            {
                LoanId = loan.LOAN_ID,
                LoanType = loan.LoanTypes.TYPES_NAME,
                LenderId = loan.ID_LENDER,
                StudentCode = code,
                Book = new LoanedBookApiResponse()
                {
                    BookId = loan.Books.BOOK_ID,
                    Title = loan.Books.TITLE,
                    Category = loan.Books.Categories.CATEGORY_NAME
                },
                ReservationStatus = loan.ReservationStatus.STATUS_NAME,
                Fee = loan.FEE,
                Status = loan.STATUS,
                Date = new LoanedDateApiResponse()
                {
                    LoanDateId = loansDates.FirstOrDefault(x=>x.ID_LOAN == loan.LOAN_ID).LOAN_DATE_ID,
                    StartDate = loansDates.FirstOrDefault(x=>x.ID_LOAN == loan.LOAN_ID).START_DATE,
                    EndDate = loansDates.FirstOrDefault(x=>x.ID_LOAN == loan.LOAN_ID).END_DATE
                },
                RegistrationDate = loan.REGISTRATION_DATE,
                DaysToReturn = days >= 0 ? days2return : 0,
                DaysLate = days < 0 ? daysLate : 0,
                IsExpired = days < 0
            };

            loansResponse.Add(loanedApiResponse);
        }
        
        return Ok(new {Loans=loansResponse, Success=true});
    }

    [HttpGet("expired/{code}")]
    public async Task<IActionResult> GetExpiredLoansByStudentCode(string code)
    {
        Student student = await _loanService.GetStudentByCode(code);
        var (ExpiredLoans, ExpiredLoansDates) = await _loanService.GetStudentExpiredLoans(student);

        if(student == null || student.StudentCode == "" || ExpiredLoans.Count() == 0 || ExpiredLoansDates.Count() == 0)
        {
            return NotFound(new {Loans=new{}, Success=false}); 
        }

        List<LoanedApiResponse> loansResponse = new List<LoanedApiResponse>();

        foreach(var loan in ExpiredLoans)
        {
            var loandate = ExpiredLoansDates.Find(x=>x.ID_LOAN == loan.LOAN_ID);
            int days = 0;
            int days2return = 0;
            int daysLate = 0;
            long dateId = 0;
            DateTime startDate = DateTime.Now;
            DateTime endDate = DateTime.Now;

            if(loandate != null)
            {   
                days = (loandate.END_DATE - DateTime.Now).Days;
                days2return = days + 1;
                daysLate = Math.Abs(days);
                dateId = loandate.LOAN_DATE_ID;
                startDate = loandate.START_DATE;
                endDate = loandate.END_DATE;
            }

            LoanedApiResponse loanedApiResponse = new LoanedApiResponse()
            {
                LoanId = loan.LOAN_ID,
                LoanType = loan.LoanTypes.TYPES_NAME,
                LenderId = loan.ID_LENDER,
                StudentCode = code,
                Book = new LoanedBookApiResponse()
                {
                    BookId = loan.Books.BOOK_ID,
                    Title = loan.Books.TITLE,
                    Category = loan.Books.Categories.CATEGORY_NAME
                },
                ReservationStatus = loan.ReservationStatus.STATUS_NAME,
                Fee = loan.FEE,
                Status = loan.STATUS,
                Date = new LoanedDateApiResponse()
                {
                    LoanDateId = dateId,
                    StartDate = startDate,
                    EndDate = endDate
                },
                RegistrationDate = loan.REGISTRATION_DATE,
                DaysToReturn = days >= 0 ? days2return : 0,
                DaysLate = days < 0 ? daysLate : 0,
                IsExpired = days < 0
            };
            loansResponse.Add(loanedApiResponse);
        }

        return Ok(new {Loans=loansResponse, Success=true});
    }

}