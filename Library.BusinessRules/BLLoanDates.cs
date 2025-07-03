using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.DataAccess.Domain;
using Library.DataAccess.Repositories;

namespace Library.BusinessRules
{
    public class BLLoanDates
    {
        public async Task<int> CreateLoanDatesAsync(LoanDates pLoanDates)
        {
            return await DALLoanDates.CreateLoandDateAsync(pLoanDates);
        }
        public async Task<int> UpdateLoanDatesAsync(LoanDates pLoandDates)
        {
            return await DALLoanDates.UpdateLoandDatesAsync(pLoandDates);
        }
        public async Task<LoanDates> GetLoanDatessByIdAsync(LoanDates pLoanDates)
        {
            return await DALLoanDates.GetLoanDatesByIdAsync(pLoanDates);
        }
        public async Task<List<LoanDates2>> GetLoanDatesByIdLoanAsync(LoanDates pLoanDates)
        {
            return await DALLoanDates.GetLoanDatesByIdLoanAsync(pLoanDates);
        }
        public async Task<List<LoanDates>> GetAllLoanDatesAsync()
        {
            return await DALLoanDates.GetAllLoanDatesAsync();
        }
        public async Task<List<LoanDates2>> GetExpiredDatesAsync()
        {
            return await DALLoanDates.GetExpiredDatesAsync();
        }
        public async Task<List<LoanDates2>> GetExpiredDatesByIdLoanAsync(Loans pLoan)
        {
            return await DALLoanDates.GetExpiredDatesByIdLoanAsync(pLoan);
        }
        public async Task<List<LoanDates2>> GetDatesToExpireSoonAsync()
        {
            return await DALLoanDates.GetDatesToExpireSoonAsync();
        }
    }
}
