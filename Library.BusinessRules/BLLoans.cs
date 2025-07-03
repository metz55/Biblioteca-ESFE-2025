using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.DataAccess.Domain;
using Library.DataAccess.Repositories;

namespace Library.BusinessRules
{
    public class BLLoans
    {
        //En los metodos siguientes  se devuelven las funcionalidades programada en los metodos DAL
        public async Task<int> CreateLoansAsync(Loans pLoans)
        {
            return await DALLoans.CreateLoansAsync(pLoans);
        }
        public async Task<int> UpdateLoansAsync(Loans pLoans)
        {
            return await DALLoans.UpdateLoansAsync(pLoans);
        }
        public async Task<int> UpdateLoans02Async(Loans2 pLoans)
        {
            return await DALLoans.UpdateLoans02Async(pLoans);
        }
        public async Task<int> DeleteLoansAsync(Loans pLoans)
        {
            return await DALLoans.DeleteLoanssAsync(pLoans);
        }
        public async Task<Loans> GetLoansByIdAsync(Loans pLoans)
        {
            return await DALLoans.GetLoansByIdAsync(pLoans);
        }
        public async Task<List<Loans>> GetAllLoansAsync()
        {
            return await DALLoans.GetAllLoansAsync();
        }
        public async Task<List<Loans>> GetLoansAsync(Loans pLoans)
        {
            return await DALLoans.GetLoansAsync(pLoans);
        }
        public async Task<List<Loans>> GetIncludePropertiesAsync(Loans pLoans)
        {
            return await DALLoans.GetIncludePropertiesAsync(pLoans);
        }
        public async Task<Loans> GetLastLoan(Loans pLoans)
        {
            return await DALLoans.GetLastLoan(pLoans);
        }

        public async Task<List<Loans>> GetLoanByIdLender(long IdLender)
        {
            return await DALLoans.GetLoanByIdLender(IdLender);
        }
        public async Task<List<Loans>> GetExpiredLoansByIdLenderAsync(Loans pLoan)
        {
            return await DALLoans.GetExpiredLoansByIdLenderAsync(pLoan);
        }
        public async Task<List<Loans>> GetExpiredLoansAsync(List<LoanDates2> loanDates)
        {
            return await DALLoans.GetExpiredLoansAsync(loanDates);
        }
    }
}
