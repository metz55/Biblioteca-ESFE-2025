using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.DataAccess.Domain;
using Library.DataAccess.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Library.DataAccess.Repositories
{
    public class DALLoans
    {
        #region CRUD
        public static async Task<int> CreateLoansAsync(Loans pLoans)
        {
            int result = 0;
            using (var dbContext = new DBContext())
            {
                dbContext.Add(pLoans);
                result = await dbContext.SaveChangesAsync();
            }
            return result;
        }

        public static async Task<int> UpdateLoansAsync(Loans pLoans)
        {
            int result = 0;
            using (var dbContext = new DBContext())
            {
                var loans = await dbContext.Loans.FirstOrDefaultAsync(s => s.LOAN_ID == pLoans.LOAN_ID);
                loans.ID_TYPE = pLoans.ID_TYPE;
                loans.ID_RESERVATION = pLoans.ID_RESERVATION;
                loans.COPY = pLoans.COPY;
                loans.FEE = pLoans.FEE;
                loans.LENDER_CONTACT = pLoans.LENDER_CONTACT;
                loans.STATUS = pLoans.STATUS;

                dbContext.Update(loans);
                result = await dbContext.SaveChangesAsync();
            }
            return result;
        }

        public static async Task<int> UpdateLoans02Async(Loans2 pLoans)
        {
            int result = 0;
            using (var dbContext = new DBContext())
            {
                var loans = await dbContext.Loans.FirstOrDefaultAsync(s => s.LOAN_ID == pLoans.LOAN_ID);
                loans.STATUS = pLoans.STATUS;
                dbContext.Update(loans);
                result = await dbContext.SaveChangesAsync();
            }
            return result;
        }


        public static async Task<int> DeleteLoanssAsync(Loans pLoans)
        {
            int result = 0;
            using (var dbContext = new DBContext())
            {
                var loans = await dbContext.Loans.FirstOrDefaultAsync(s => s.LOAN_ID == pLoans.LOAN_ID);
                dbContext.Loans.Remove(loans);
                result = await dbContext.SaveChangesAsync();
            }
            return result;
        }

        public static async Task<List<Loans>> GetAllLoansAsync()
        {
            var loans = new List<Loans>();
            using (var dbContext = new DBContext())
            {
                loans = await dbContext.Loans.ToListAsync();
            }
            return loans;
        }

        public static async Task<List<Loans>> GetExpiredLoansAsync(List<LoanDates2> loanDates)
        {
            var loanIds = loanDates.Select(ld => ld.ID_LOAN).ToList();
            using (var dbContext = new DBContext())
            {
                var loans = await dbContext.Loans
                    .Include(l => l.Books)
                    .Include(l => l.LoanTypes)
                    .Where(l => loanIds.Contains(l.LOAN_ID) && l.ID_RESERVATION == 1)
                    .ToListAsync();
                
                return loans;
            }
        }

        public static async Task<List<Loans>> GetExpiredLoansByIdLenderAsync(Loans pLoan)
        {
            // var loanIds = loanDates.Select(ld => ld.ID_LOAN).ToList();
            using (var dbContext = new DBContext())
            {
                var loans = await dbContext.Loans
                    .Include(l => l.Books).ThenInclude(b=>b.Categories)
                    .Include(l => l.LoanTypes)
                    .Include(l => l.ReservationStatus)
                    .Where(l => l.ID_LENDER == pLoan.ID_LENDER && l.ID_RESERVATION == 1 || l.ID_RESERVATION == 6)
                    .ToListAsync();
                
                return loans;
            }
        }

        public static async Task<Loans> GetLoansByIdAsync(Loans pLoans)
        {
            var loans = new Loans();
            using (var dbContext = new DBContext())
            {
                loans = await dbContext.Loans.FirstOrDefaultAsync(s => s.LOAN_ID == pLoans.LOAN_ID);
            }
            return loans;
        }

        internal static IQueryable<Loans> QuerySelect(IQueryable<Loans> pQuery, Loans pLoans)
        {
            if (pLoans.LOAN_ID > 0)
                pQuery = pQuery.Where(s => s.LOAN_ID == pLoans.LOAN_ID);

            if (pLoans.ID_TYPE > 0)
                pQuery = pQuery.Where(s => s.ID_TYPE == pLoans.ID_TYPE);

            if (pLoans.ID_RESERVATION > 0)
                pQuery = pQuery.Where(s => s.ID_RESERVATION == pLoans.ID_RESERVATION);

            if (pLoans.ID_BOOK > 0)
                pQuery = pQuery.Where(s => s.ID_BOOK == pLoans.ID_BOOK);

            if (pLoans.ID_LENDER > 0)
                pQuery = pQuery.Where(s => s.ID_LENDER == pLoans.ID_LENDER);

            if (pLoans.STATUS is true)
                pQuery = pQuery.Where(s => s.STATUS == pLoans.STATUS);

            if (!string.IsNullOrWhiteSpace(pLoans.LENDER_CONTACT))
                pQuery = pQuery.Where(s => s.LENDER_CONTACT.Contains(pLoans.LENDER_CONTACT));

            pQuery = pQuery.OrderByDescending(s => s.LOAN_ID).AsQueryable();
            if (pLoans.Top_Aux > 0)
                pQuery = pQuery.Take(pLoans.Top_Aux).AsQueryable();
            return pQuery;
        }

        public static async Task<List<Loans>> GetLoansAsync(Loans pLoans)
        {
            var loans = new List<Loans>();
            using (var dbContext = new DBContext())
            {
                var select = dbContext.Loans.AsQueryable();
                select = QuerySelect(select, pLoans);
                loans = await select.ToListAsync();
            }
            return loans;
        }

        public static async Task<List<Loans>> GetIncludePropertiesAsync(Loans pLoans)
        {
            var loans = new List<Loans>();
            using (var bdContexto = new DBContext())
            {
                var select = bdContexto.Loans.AsQueryable();
                select = QuerySelect(select, pLoans).Include(e => e.LoanTypes).AsQueryable();
                select = QuerySelect(select, pLoans).Include(e => e.ReservationStatus).AsQueryable();
                select = QuerySelect(select, pLoans).Include(e => e.Books).AsQueryable();

                loans = await select.ToListAsync();
            }

            return loans;

        }

        public static async Task<Loans> GetLastLoan(Loans pLoans)
        {
            var bdContexto = new DBContext();

            var lastLoan = bdContexto.Loans.OrderByDescending(x => x.LOAN_ID).FirstOrDefault();

            return lastLoan;
        }

        public static async Task<List<Loans>> GetLoanByIdLender(long id)
        {
            var loans = new List<Loans>();
            using (var dbContext = new DBContext())
            {
                loans = await dbContext.Loans
                    .Include(l => l.Books)
                    .Include(l => l.LoanTypes)
                    .Include(l => l.ReservationStatus)
                    .Where(l => l.ID_LENDER == id && l.ID_RESERVATION == 1 || l.ID_RESERVATION == 4)
                    .ToListAsync();
            }
            return loans;
        }
        #endregion

    }
}
